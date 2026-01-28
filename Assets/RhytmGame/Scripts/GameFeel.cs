using UnityEngine;
using DG.Tweening;

public class GameFeel : MonoBehaviour
{
    public static GameFeel Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ShapeController playerShape;
    [SerializeField] private ShapeController targetShape;
    [SerializeField] private SpriteRenderer flashOverlay;
    [SerializeField] private LockMechanic lockMechanic;
    [SerializeField] private RoundManager roundManager;

    [Header("Punch Effect")]
    [SerializeField] private float perfectPunchScale = 1.3f;
    [SerializeField] private float goodPunchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private int punchVibrato = 5;

    [Header("Flash Effect")]
    [SerializeField] private bool enableFlash = true;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float flashAlpha = 0.3f;
    [SerializeField] private Color perfectFlashColor = Color.white;
    [SerializeField] private Color goodFlashColor = new Color(1f, 1f, 0.3f);
    [SerializeField] private Color missFlashColor = new Color(1f, 0.3f, 0.3f);

    [Header("Round Transition")]
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private Ease appearEase = Ease.OutBack;

    [Header("Particles")]
    [SerializeField] private bool enableParticles = true;
    [SerializeField] private ParticleSystem perfectParticles;
    [SerializeField] private ParticleSystem goodParticles;
    [SerializeField] private ParticleSystem missParticles;

    private Vector3 playerInitialScale;
    private Vector3 targetInitialScale;
    private bool scalesCaptured;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (roundManager == null)
            roundManager = RoundManager.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += HandleLock;
        }

        if (roundManager != null)
        {
            roundManager.OnRoundStart += HandleRoundStart;
        }

        CreateFlashOverlayIfNeeded();
        CreateParticlesIfNeeded();
    }

    private void LateUpdate()
    {
        if (!scalesCaptured)
        {
            if (playerShape != null)
                playerInitialScale = playerShape.transform.localScale;
            if (targetShape != null)
                targetInitialScale = targetShape.transform.localScale;
            scalesCaptured = true;
        }
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }

        if (roundManager != null)
        {
            roundManager.OnRoundStart -= HandleRoundStart;
        }
    }

    private void HandleLock(LockResult result)
    {
        switch (result)
        {
            case LockResult.Perfect:
                PunchShape(perfectPunchScale);
                Flash(perfectFlashColor);
                PlayParticles(perfectParticles);
                break;

            case LockResult.Good:
                PunchShape(goodPunchScale);
                Flash(goodFlashColor);
                PlayParticles(goodParticles);
                break;

            case LockResult.Miss:
                Flash(missFlashColor);
                PlayParticles(missParticles);
                break;
        }
    }

    private void HandleRoundStart(int round)
    {
        if (round > 1)
        {
            AnimateShapeAppear();
        }
    }

    private void PunchShape(float scale)
    {
        if (playerShape == null)
            return;

        playerShape.transform.DOKill();
        playerShape.transform.localScale = playerInitialScale;

        playerShape.transform.DOPunchScale(
            playerInitialScale * (scale - 1f),
            punchDuration,
            punchVibrato,
            0.5f
        ).OnComplete(() =>
        {
            var rotationController = playerShape.GetComponent<RotationController>();
            if (rotationController != null)
            {
                rotationController.RecaptureScale();
            }
        });
    }

    private void Flash(Color color)
    {
        if (!enableFlash || flashOverlay == null)
            return;

        flashOverlay.DOKill();
        color.a = flashAlpha;
        flashOverlay.color = color;

        flashOverlay.DOFade(0f, flashDuration).SetEase(Ease.OutQuad);
    }

    private void PlayParticles(ParticleSystem particles)
    {
        if (!enableParticles || particles == null)
            return;

        if (playerShape != null)
        {
            particles.transform.position = playerShape.transform.position;
        }

        particles.Stop();
        particles.Play();
    }

    private void AnimateShapeAppear()
    {
        if (playerShape != null)
        {
            playerShape.transform.localScale = Vector3.zero;
            playerShape.transform.DOScale(playerInitialScale, appearDuration)
                .SetEase(appearEase)
                .OnComplete(() =>
                {
                    var rotationController = playerShape.GetComponent<RotationController>();
                    if (rotationController != null)
                    {
                        rotationController.RecaptureScale();
                    }
                });
        }

        if (targetShape != null)
        {
            targetShape.transform.localScale = Vector3.zero;
            targetShape.transform.DOScale(targetInitialScale, appearDuration)
                .SetEase(appearEase);
        }
    }

    private void CreateFlashOverlayIfNeeded()
    {
        if (flashOverlay != null)
            return;

        GameObject flashGO = new GameObject("FlashOverlay");
        flashGO.transform.SetParent(transform);
        flashGO.transform.position = Vector3.zero;

        flashOverlay = flashGO.AddComponent<SpriteRenderer>();
        flashOverlay.sprite = CreateWhiteSprite();
        flashOverlay.sortingOrder = 100;
        flashOverlay.color = new Color(1, 1, 1, 0);

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;
        flashGO.transform.localScale = new Vector3(camWidth + 2f, camHeight + 2f, 1f);
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D texture = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    private void CreateParticlesIfNeeded()
    {
        if (perfectParticles == null)
            perfectParticles = CreateParticleSystem("PerfectParticles", new Color(0.3f, 1f, 0.3f));

        if (goodParticles == null)
            goodParticles = CreateParticleSystem("GoodParticles", new Color(1f, 1f, 0.3f));

        if (missParticles == null)
            missParticles = CreateParticleSystem("MissParticles", new Color(1f, 0.3f, 0.3f));
    }

    private ParticleSystem CreateParticleSystem(string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.position = Vector3.zero;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 0.2f;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 30;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 50;

        ps.Stop();

        return ps;
    }

    public void SetReferences(ShapeController player, ShapeController target)
    {
        playerShape = player;
        targetShape = target;
        scalesCaptured = false;
    }

    public void RecaptureScales()
    {
        scalesCaptured = false;
    }
}
