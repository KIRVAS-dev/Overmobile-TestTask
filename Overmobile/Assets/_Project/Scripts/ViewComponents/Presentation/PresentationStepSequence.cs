using Core.Gameplay.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExtendedExceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace ViewComponents.Presentation
{
    [DisallowMultipleComponent]
    public sealed class PresentationStepSequence : MonoBehaviour
    {
        [SerializeField] private bool _playOnEnable;
        [SerializeField] private bool _playOnStart;
        [SerializeField] private bool _stopOnDisable;
        [SerializeReference] private PresentationStepDefinition[] _steps = Array.Empty<PresentationStepDefinition>();

        private readonly List<Tween> _activeTweens = new List<Tween>();

        private IActiveCharacterPresentationProvider _activeCharacterPresentationProvider;
        private IActivePresentationSectionMapProvider _activePresentationSectionMapProvider;
        private IPlayerUpgradeService _playerUpgradeService;
        private CancellationTokenSource _playCancellationTokenSource;
        private UniTaskCompletionSource _runCompletionSource;
        private bool _isPlaying;

        [Inject]
        public void Construct(
            IActiveCharacterPresentationProvider activeCharacterPresentationProvider,
            IActivePresentationSectionMapProvider activePresentationSectionMapProvider,
            IPlayerUpgradeService playerUpgradeService)
        {
            _activeCharacterPresentationProvider = activeCharacterPresentationProvider;
            _activePresentationSectionMapProvider = activePresentationSectionMapProvider;
            _playerUpgradeService = playerUpgradeService;
        }

        public void StartPresentationFireAndForget(CancellationToken cancellationToken)
        {
            if (_isPlaying)
            {
                return;
            }

            StartPresentationAsync(cancellationToken).Forget();
        }

        public async UniTask StartPresentationAsync(CancellationToken cancellationToken)
        {
            if (_isPlaying)
            {
                if (_runCompletionSource != null)
                {
                    await _runCompletionSource.Task.AttachExternalCancellation(cancellationToken);
                }

                return;
            }

            UniTaskCompletionSource runCompletionSource = new UniTaskCompletionSource();
            _runCompletionSource = runCompletionSource;

            _playCancellationTokenSource?.Cancel();
            _playCancellationTokenSource?.Dispose();

            _playCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                this.GetCancellationTokenOnDestroy(),
                cancellationToken
            );

            _isPlaying = true;

            try
            {
                await RunPresentationAsync(_playCancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                _isPlaying = false;
                KillActiveTweens();
                _playCancellationTokenSource?.Dispose();
                _playCancellationTokenSource = null;
                runCompletionSource.TrySetResult();

                if (_runCompletionSource == runCompletionSource)
                {
                    _runCompletionSource = null;
                }
            }
        }

        internal void RegisterTween(Tween tween)
        {
            if (tween == null)
            {
                throw new InvalidPresentationStepSequenceException(gameObject.name, "Tween is not assigned");
            }

            _activeTweens.Add(tween);
        }

        private async UniTask RunPresentationAsync(CancellationToken cancellationToken)
        {
            PresentationContext context = new PresentationContext(
                this,
                _activeCharacterPresentationProvider,
                _activePresentationSectionMapProvider,
                _playerUpgradeService,
                cancellationToken
            );

            for (int i = 0; i < _steps.Length; i++)
            {
                PresentationStepDefinition step = _steps[i];

                if (step == null)
                {
                    throw new InvalidPresentationStepSequenceException(gameObject.name, $"Step at index {i} is missing");
                }

                await step.ExecuteAsync(context, cancellationToken);
            }
        }

        private void StartPresentation()
        {
            StartPresentationAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void StopPresentation()
        {
            if (!_isPlaying
             && _playCancellationTokenSource == null)
            {
                return;
            }

            _playCancellationTokenSource?.Cancel();
            KillActiveTweens();
            _isPlaying = false;
            _runCompletionSource?.TrySetResult();
            _runCompletionSource = null;
        }

        private void Awake()
        {
            Validate();
        }

        private void Start()
        {
            if (_playOnStart)
            {
                StartPresentation();
            }
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                StartPresentation();
            }
        }

        private void OnDisable()
        {
            if (_stopOnDisable)
            {
                StopPresentation();
            }
        }

        private void OnDestroy()
        {
            _playCancellationTokenSource?.Cancel();
            KillActiveTweens();
        }

        private void KillActiveTweens()
        {
            foreach (Tween tween in _activeTweens)
            {
                if (tween != null
                 && tween.IsActive())
                {
                    tween.Kill();
                }
            }

            _activeTweens.Clear();
        }

        private void Validate()
        {
            Guard.AgainstNullOrEmpty(
                _steps,
                () => new InvalidPresentationStepSequenceException(gameObject.name, "Steps are not assigned")
            );
        }
    }
}
