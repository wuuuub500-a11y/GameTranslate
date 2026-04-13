using UnityEngine;

public class FinalResultManager : MonoBehaviour
{
    [Header("胜利图片")]
    public GameObject playerAWinImage;
    public GameObject playerBWinImage;

    [Header("胜利BGM")]
    public AudioSource bgmAudioSource;
    public AudioClip playerAWinBgm;
    public AudioClip playerBWinBgm;

    private void Start()
    {
        // 先全部隐藏
        if (playerAWinImage != null)
            playerAWinImage.SetActive(false);

        if (playerBWinImage != null)
            playerBWinImage.SetActive(false);

        // 根据总比分显示赢家图片并播放对应BGM
        if (GameData.playerAWins > GameData.playerBWins)
        {
            if (playerAWinImage != null)
                playerAWinImage.SetActive(true);

            PlayBgm(playerAWinBgm);
        }
        else
        {
            // 由于总局数是5局，这里理论上就是B赢
            if (playerBWinImage != null)
                playerBWinImage.SetActive(true);

            PlayBgm(playerBWinBgm);
        }
    }

    void PlayBgm(AudioClip clip)
    {
        if (bgmAudioSource != null && clip != null)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }
    }
}