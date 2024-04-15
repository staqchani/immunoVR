using UnityEngine;
using UnityEngine.UI;
using Platinio.TweenEngine;
using VRBeats.ScriptableEvents;
using System.Collections.Generic;

namespace VRBeats
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private Text multiplierLabel = null;
        [SerializeField] private Text scoreLabel = null;
        [SerializeField] private Text HintLabel = null;
        [SerializeField] private Image multiplierLoader = null;
        [SerializeField] private float scoreFollowTime = 1.0f;
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private GameEvent onGameOver = null;
        [SerializeField] private GameObject[] Lives;
        private int lives;
        private int maxMultiplier = 0;
        private int scorePerHit = 0;
        private int currentScore = 0;
        private int currentMultiplier = 0;
        private int toNextMultiplierIncrease = 2;
        private int acumulateCorrectSlices = 0;
        private int acumulateErrors = 0;
        private int errorLimit = 0;
        private float visualScore = 0.0f;
        private int scoreTweenID = -1;
        private int loaderTweenID = -1;

        private bool destroyed = false;

        Queue<int> cubesQueue = new Queue<int>();
        public int CurrentScore
        {
            get
            {
                return currentScore;
            }
        }

        [SerializeField] GameEvent onLeftCubeEnter;
        [SerializeField] GameEvent onRightCubeEnter;
        private void Awake()
        {
            maxMultiplier = VR_BeatManager.instance.GameSettings.MaxMultiplier;
            scorePerHit = VR_BeatManager.instance.GameSettings.ScorePerHit;
            errorLimit = VR_BeatManager.instance.GameSettings.ErrorLimit;
            multiplierLoader.fillAmount = 0.0f;
            lives = Lives.Length;
        }       

        public void OnGameOver()
        {            
            gameObject.CancelAllTweens();
            canvasGroup.Fade(0.0f, 0.5f).SetEase(Ease.EaseOutExpo).SetOwner(gameObject);
        }

        public void OnGameRestart()
        {            
            ResetThisComponent();
            gameObject.CancelAllTweens();
            canvasGroup.Fade(1.0f, 0.5f).SetEase(Ease.EaseOutExpo).SetOwner(gameObject);
        }

        public void ResetThisComponent()
        {           
            currentMultiplier = 0;
            currentScore = 0;
            acumulateCorrectSlices = 0;
            visualScore = 0;
            acumulateErrors = 0;
            toNextMultiplierIncrease = 3;
            lives = Lives.Length;
            foreach (var item in Lives)
            {
                item.SetActive(true);
            }
        }



        private void Update()
        {
            UpdateUI();
        }

        public void OnMineSlice()
        {
            lives--;
            if (lives > 0) Lives[lives - 1].SetActive(false);
            acumulateCorrectSlices = 0;
            currentMultiplier = 0;
            toNextMultiplierIncrease = 3;
            currentScore -= scorePerHit + (scorePerHit * currentMultiplier);
            acumulateErrors = 0;
            UpdateMultiplierLoaderValue();
            CheckGame();
            //OnIncorrectSlice();
        }
        public void OnCorrectSlice()
        {
            if (destroyed)
                return;
            ShowNextHint();
            acumulateErrors = 0;
            acumulateCorrectSlices++;
            currentScore += scorePerHit + (scorePerHit * currentMultiplier);

            CancelTweenById(scoreTweenID);
            scoreTweenID = PlatinioTween.instance.ValueTween(visualScore, currentScore, scoreFollowTime).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float value)
             {                
                 visualScore = value;
             }).ID;

            UpdateMultiplierLoaderValue();

            if (acumulateCorrectSlices >= toNextMultiplierIncrease)
            {
                IncreaseMultiplier();
            }


        }

        private void CancelTweenById(int id)
        {
            if(id != -1)
                PlatinioTween.instance.CancelTween(id);
        }

        private void UpdateMultiplierLoaderValue()
        {
            if (destroyed)
                return;

            float multiplierLoaderValue = (float)acumulateCorrectSlices / (float)toNextMultiplierIncrease;


            CancelTweenById(loaderTweenID);
            loaderTweenID = PlatinioTween.instance.ValueTween(multiplierLoader.fillAmount, multiplierLoaderValue, 1.0f).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float value)
            {
                if(multiplierLoader != null)
                    multiplierLoader.fillAmount = value;
            }).SetOwner(multiplierLoader.gameObject).ID;
        }

        
        public void OnIncorrectSlice()
        {
            if (destroyed)
                return;
            ShowNextHint();
            acumulateErrors++;
            acumulateCorrectSlices = 0;
            currentMultiplier = 0;
            toNextMultiplierIncrease = 3;
            currentScore -= scorePerHit + (scorePerHit * currentMultiplier);
            CancelTweenById(scoreTweenID);
            scoreTweenID = PlatinioTween.instance.ValueTween(visualScore, currentScore, scoreFollowTime).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float value)
            {
                visualScore = value;
            }).ID;
            UpdateMultiplierLoaderValue();

            if (acumulateErrors > errorLimit)
            {
                acumulateErrors = 0;
                lives--;
                if (lives > 0) Lives[lives - 1].SetActive(false);
                CheckGame();
                //onGameOver.Invoke();
            }

        }

        void CheckGame()
        {
            if (lives == 0) onGameOver.Invoke();
        }
        public void OnHintUpdate(string text)
        {
            int raket = text.ToLower().Equals("left") ? 1 : 0;
            cubesQueue.Enqueue(raket);
            if (cubesQueue.Count == 1) ShowNextHint();
            //HintLabel.text = text;
        }
        public void ShowNextHint()
        {
            if (cubesQueue.Count == 0) return;
            int raket = cubesQueue.Dequeue();
            HintLabel.text = raket == 1 ? "Left" : "Right";

            if (raket == 1) onLeftCubeEnter?.Invoke();
            else onRightCubeEnter?.Invoke();
        }
        private void UpdateUI()
        {
            if (destroyed)
                return;

            multiplierLabel.text = currentMultiplier.ToString();
            scoreLabel.text = Mathf.CeilToInt( visualScore ).ToString();
        }

        private void IncreaseMultiplier()
        {
            if (destroyed)
                return;

            acumulateCorrectSlices = 0;
            currentMultiplier = Mathf.Min( currentMultiplier + 1 , maxMultiplier );
            toNextMultiplierIncrease = (currentMultiplier + 1) * 2;

            PlatinioTween.instance.CancelTween(multiplierLoader.gameObject);

            PlatinioTween.instance.ValueTween(multiplierLoader.fillAmount, 1.0f, 1.0f).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float value)
            {
                if(multiplierLoader != null)
                    multiplierLoader.fillAmount = value;
            }).SetOwner(multiplierLoader.gameObject).SetOnComplete( delegate 
            {
                if (multiplierLabel != null)
                {
                    PlatinioTween.instance.ValueTween(multiplierLoader.fillAmount, 0.0f, 0.5f).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float value)
                    {
                        if (multiplierLoader != null)
                            multiplierLoader.fillAmount = value;
                    }).SetOwner(multiplierLoader.gameObject);
                }
                
            } );

        }

    }

}

