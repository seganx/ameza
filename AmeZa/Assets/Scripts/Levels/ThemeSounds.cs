using UnityEngine;

public class ThemeSounds : MonoBehaviour
{
    [SerializeField] private AudioSource[] hitSounds = null;
    [SerializeField] private AudioSource[] breakSounds = null;
    [SerializeField] private AudioSource[] releaseBallSounds = null;
    [SerializeField] private AudioSource[] damageSounds = null;

    public void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.OnBlockHit:
                {
                    if (param.Is<BlockBase>())
                        DispatchOnBlockHit(param.As<BlockBase>().Type);
                }
                break;

            case Messages.Type.OnBlockDeath:
                {
                    if (param.Is<BlockBase>())
                        DispatchOnBlockDeath(param.As<BlockBase>().Type);
                }
                break;
        }
    }

    private void DispatchOnBlockHit(BlockType type)
    {
        switch (type)
        {
            case BlockType.Obstacle: PlayHit(); break;
            case BlockType.BoxKill: break;
            case BlockType.HorizontalKill: break;
            case BlockType.VerticalKill: break;
            case BlockType.CrossKill: break;
            case BlockType.HorizontalDamage: PlayDamage(); break;
            case BlockType.VerticalDamage: PlayDamage(); break;
            case BlockType.CrossDamage: PlayDamage(); break;
            case BlockType.Ball: break;
            case BlockType.Null: break;
            case BlockType.RandomValue: break;
            case BlockType.Value: PlayHit(); break;
        }
    }

    private void DispatchOnBlockDeath(BlockType type)
    {
        switch (type)
        {
            case BlockType.Obstacle: PlayBreak(); break;
            case BlockType.BoxKill: break;
            case BlockType.HorizontalKill: break;
            case BlockType.VerticalKill: break;
            case BlockType.CrossKill: break;
            case BlockType.HorizontalDamage: PlayDamage(); PlayBreak(); break;
            case BlockType.VerticalDamage: PlayDamage(); PlayBreak(); break;
            case BlockType.CrossDamage: PlayDamage(); PlayBreak(); break;
            case BlockType.Ball: PlayReleaseBall(); break;
            case BlockType.Null: break;
            case BlockType.RandomValue: break;
            case BlockType.Value: PlayBreak(); break;
        }
    }

    private void PlayHit()
    {
        if (CanNotPlay) return;
        playingCount++;
        var index = Random.Range(0, 1000);
        var source = hitSounds[index % hitSounds.Length];
        source.PlayOneShot(source.clip, 0.75f);
    }

    private void PlayBreak()
    {
        if (CanNotPlay) return;
        playingCount++;
        var index = Random.Range(0, 1000);
        var source = breakSounds[index % breakSounds.Length];
        source.PlayOneShot(source.clip);
    }

    private void PlayReleaseBall()
    {
        if (CanNotPlay) return;
        playingCount++;
        var index = Random.Range(0, 1000);
        var source = releaseBallSounds[index % releaseBallSounds.Length];
        source.PlayOneShot(source.clip);
    }

    private void PlayDamage()
    {
        if (CanNotPlay) return;
        playingCount++;
        var index = Random.Range(0, 1000);
        var source = damageSounds[index % releaseBallSounds.Length];
        source.PlayOneShot(source.clip);
    }

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static float playingCount = 0;
    private static float maxPlayingCount = 5;

    private static bool CanNotPlay => playingCount > maxPlayingCount;

    public static void UpdateMe(float deltaTime)
    {
        playingCount = Mathf.Max(0, playingCount - deltaTime * 2);
    }
}
