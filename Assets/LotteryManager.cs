using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class LotteryManager : MonoBehaviour
{
    public static LotteryManager Instance { get; private set; }

    [Header("��d���O")]
    public GameObject lotteryPanel;          // ��d�D���O
    public GameObject drawResultPanel;       // ��d���G���O

    [Header("�귽���")]
    public TMP_Text coinsText;              // �����ƶq
    public TMP_Text diamondsText;           // �p�ۼƶq

    [Header("��d���s")]
    public Button normalDrawButton;          // ���q������s (500����)
    public Button premiumDrawButton;         // ���ũ�����s (3�p��)
    public Button closeResultButton;         // �������G���O���s

    [Header("�d���Ϥ�")]
    public Image resultCardImage;            // ��d���G��ܪ��Ϥ�
    public Sprite cardBackSprite;           // �d���I���Ϥ�

    [Header("�d���Ϥ��]�m")]
    public Sprite goblinSprite;             // �����L
    public Sprite athenaSprite;             // ����R
    public Sprite pirateSprite;             // ���s
    public Sprite teacherSprite;            // �����Ѯv
    public Sprite santaSprite;              // �t�ϦѤ���
    public Sprite popeSprite;               // �Щv

    [Header("���ܭ��O")]
    public GameObject insufficientFundsPanel;    // �l�B�������ܭ��O

    private Dictionary<string, Sprite> cardSprites;
    private Sprite drawnCardSprite;         // �Ȧs��쪺�d���Ϥ�

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCardSprites();
    }

    void InitializeCardSprites()
    {
        cardSprites = new Dictionary<string, Sprite>
        {
            { "�����L", goblinSprite },
            { "����R", athenaSprite },
            { "���s", pirateSprite },
            { "�����Ѯv", teacherSprite },
            { "�t�ϦѤ���", santaSprite },
            { "�Щv", popeSprite }
        };
    }

    void Start()
    {
        normalDrawButton.onClick.AddListener(() => StartCoroutine(OnNormalDraw()));
        premiumDrawButton.onClick.AddListener(() => StartCoroutine(OnPremiumDraw()));
        closeResultButton.onClick.AddListener(CloseResultPanel);

        drawResultPanel.SetActive(false);
        insufficientFundsPanel.SetActive(false);
        UpdateResourceDisplay();
    }

    void OnEnable()
    {
        UpdateResourceDisplay();
    }

    public void UpdateResourceDisplay()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        int diamonds = PlayerPrefs.GetInt("Diamonds", 0);
        coinsText.text = coins.ToString();
        diamondsText.text = diamonds.ToString();
    }

    private IEnumerator OnNormalDraw()
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        if (currentCoins < 500)
        {
            insufficientFundsPanel.SetActive(true);
            yield break;
        }

        normalDrawButton.interactable = false;
        yield return StartCoroutine(APIManager.Instance.DrawCard(false));
        normalDrawButton.interactable = true;

        UpdateResourceDisplay();
    }

    private IEnumerator OnPremiumDraw()
    {
        int currentDiamonds = PlayerPrefs.GetInt("Diamonds", 0);
        if (currentDiamonds < 3)
        {
            insufficientFundsPanel.SetActive(true);
            yield break;
        }

        premiumDrawButton.interactable = false;
        yield return StartCoroutine(APIManager.Instance.DrawCard(true));
        premiumDrawButton.interactable = true;

        UpdateResourceDisplay();
    }

    public void ShowDrawResult(string cardName, string rarity)
    {
        drawResultPanel.SetActive(true);

        if (cardSprites.TryGetValue(cardName, out Sprite cardSprite))
        {
            drawnCardSprite = cardSprite;  // �Ȧs��쪺�d��
            resultCardImage.sprite = cardBackSprite;  // ����ܭI��
        }

        PlayDrawAnimation();
    }

    private void PlayDrawAnimation()
    {
        // ���m�d������m�B�Y��M����
        resultCardImage.rectTransform.localScale = Vector3.zero;
        resultCardImage.rectTransform.localRotation = Quaternion.identity;
        resultCardImage.rectTransform.anchoredPosition = new Vector2(0, -100);

        // �Ыذʵe�ǦC
        Sequence sequence = DOTween.Sequence();

        float rotationDuration = 0.6f;  // ����ʵe�`�ɶ�
        float switchImageTime = rotationDuration * 0.5f;  // �b�����@�b�ɤ����Ϥ�

        // �d���q�U��u�X�ñ���
        sequence.Append(resultCardImage.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutBack))  // �Y��ʵe
                .Join(resultCardImage.rectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack))  // �V�W����
                .Join(resultCardImage.rectTransform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360));  // ����@��

        // �b�����@�b�ɤ����쥿���Ϥ�
        sequence.InsertCallback(switchImageTime, () => {
            resultCardImage.sprite = drawnCardSprite;
        });
    }

    private void CloseResultPanel()
    {
        drawResultPanel.SetActive(false);
        resultCardImage.sprite = cardBackSprite;  // ���m���I���Ϥ�

        // ���m�d������m�B�Y��M����
        resultCardImage.rectTransform.localScale = Vector3.zero;
        resultCardImage.rectTransform.localRotation = Quaternion.identity;
        resultCardImage.rectTransform.anchoredPosition = new Vector2(0, -100);
    }
}