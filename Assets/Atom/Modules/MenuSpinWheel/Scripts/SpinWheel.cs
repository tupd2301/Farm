using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Atom.Modules.SpinWheel
{
   public sealed class SpinWheel : MonoBehaviour
   {
      private SpinWheelUI _view;
      public SpinWheelUI View { get { return _view; } }
      [SerializeField] private GameObject linePrefab;
      [SerializeField] private GameObject wheelPiecePrefab;
      
      [Header("Sounds :")]
      [SerializeField] private AudioSource audioSource;
      [SerializeField] private AudioClip tickAudioClip;
      [SerializeField][Range(0f, 1f)] private float volume = .5f;
      [SerializeField][Range(-3f, 3f)] private float pitch = 1f;

      [Space]
      [Header("Spin wheel settings :")]
      [Range(1, 20)] public int spinDuration = 8;
      public Ease spinEase = Ease.InOutQuart;

      [Space]
      [Header("Wheel pieces config :")]
      public bool DownloadPlayfabConfig = false;
      public string playfabId = "";
      public string playfabSpinWheelKey = "SpinWheelConfig";
      public WheelPiece[] wheelPieces;

      // Events
      private UnityAction onSpinStartEvent;
      private UnityAction<WheelPiece> onSpinEndEvent;


      private bool _isSpinning = false;

      public bool IsSpinning { get { return _isSpinning; } }


      private Vector2 pieceMinSize = new Vector2(81f, 146f);
      private Vector2 pieceMaxSize = new Vector2(144f, 213f);
      private int piecesMin = 2;
      private int piecesMax = 12;

      private float pieceAngle;
      private float halfPieceAngle;
      private float halfPieceAngleWithPaddings;


      private double accumulatedWeight;
      private System.Random rand = new System.Random();

      private List<int> nonZeroChancesIndices = new List<int>();

      private bool _isReady = false;

      public async Task GetConfigFromPlayfab()
      {
         if (DownloadPlayfabConfig && playfabId != "")
         {
            var task = AthenaPlayfabAPI.GetUserDataAsync(new List<string>() { playfabSpinWheelKey }, playfabId);
            await task;
            var result = task.Result;
            if (result != null)
            {
               var data = result.Data["SpinWheelConfig"].Value;
               wheelPieces = JsonConvert.DeserializeObject<WheelPiece[]>(data);
            }
            else
            {
               Debug.LogError("Failed to get SpinWheelConfig from Playfab");
            }
         }
      }

      public void Initialize()
      {
         pieceAngle = 360f / wheelPieces.Length;
         halfPieceAngle = pieceAngle / 2f;
         halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

         Generate();

         CalculateWeightsAndIndices();
         if (nonZeroChancesIndices.Count == 0)
            Debug.LogError("You can't set all pieces chance to zero");


         SetupAudio();
         _isReady = true;
      }

      public void Show()
      {
         _view = AppManager.Instance.ShowSafeTopUI<SpinWheelUI>("Atom/SpinWheelUI", false);
         _view.SpinButton.onClick.AddListener(Spin);
         if (!_isReady)
         {
            Initialize();
         }
      }

      private void SetupAudio()
      {
         audioSource.clip = tickAudioClip;
         audioSource.volume = volume;
         audioSource.pitch = pitch;
      }

      private void Generate()
      {
         wheelPiecePrefab = InstantiatePiece();

         RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
         float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
         float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

         for (int i = 0; i < wheelPieces.Length; i++)
            DrawPiece(i);

         Destroy(wheelPiecePrefab);
      }

      private void DrawPiece(int index)
      {
         WheelPiece pieceData = wheelPieces[index];
         SpinWheelPiece piece = InstantiatePiece().GetComponent<SpinWheelPiece>();
         piece.Setup(pieceData, pieceAngle / 2f, pieceAngle / 360f);

         //Line
         Transform lineTrns = Instantiate(linePrefab, View.LinesParent.position, Quaternion.identity, View.LinesParent).transform;
         lineTrns.RotateAround(View.WheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

         piece.holder.RotateAround(View.WheelPiecesParent.position, Vector3.back, pieceAngle * index);
      }

      private GameObject InstantiatePiece()
      {
         return Instantiate(wheelPiecePrefab, View.WheelPiecesParent.position, Quaternion.identity, View.WheelPiecesParent);
      }


      public void Spin()
      {
         if (!_isSpinning)
         {
            _isSpinning = true;
            if (onSpinStartEvent != null)
               onSpinStartEvent.Invoke();

            int index = GetRandomPieceIndex();
            WheelPiece piece = wheelPieces[index];

            if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0)
            {
               index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
               piece = wheelPieces[index];
            }

            float angle = -(pieceAngle * index);

            float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
            float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;

            float randomAngle = Random.Range(leftOffset, rightOffset);

            Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);

            //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
            float prevAngle, currentAngle;
            prevAngle = currentAngle = View.WheelCircle.eulerAngles.z;

            bool isIndicatorOnTheLine = false;

            View.WheelCircle
            .DORotate(targetRotation, spinDuration, RotateMode.Fast)
            .SetEase(spinEase)
            .OnUpdate(() =>
            {
               float diff = Mathf.Abs(prevAngle - currentAngle);
               if (diff >= halfPieceAngle)
               {
                  if (isIndicatorOnTheLine)
                  {
                     audioSource.PlayOneShot(audioSource.clip);
                  }
                  prevAngle = currentAngle;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine;
               }
               currentAngle = View.WheelCircle.eulerAngles.z;
            })
            .OnComplete(() =>
            {
               _view.ShowResultScreen(piece);
               _isSpinning = false;
               if (onSpinEndEvent != null)
                  onSpinEndEvent.Invoke(piece);

               onSpinStartEvent = null;
               onSpinEndEvent = null;
            });

         }
      }

      public void OnSpinStart(UnityAction action)
      {
         onSpinStartEvent = action;
      }

      public void OnSpinEnd(UnityAction<WheelPiece> action)
      {
         onSpinEndEvent = action;
      }


      private int GetRandomPieceIndex()
      {
         double r = rand.NextDouble() * accumulatedWeight;

         for (int i = 0; i < wheelPieces.Length; i++)
            if (wheelPieces[i].weight >= r)
               return i;

         return 0;
      }

      private void CalculateWeightsAndIndices()
      {
         for (int i = 0; i < wheelPieces.Length; i++)
         {
            WheelPiece piece = wheelPieces[i];

            //add weights:
            accumulatedWeight += piece.Chance;
            piece.weight = accumulatedWeight;

            //add index :
            piece.Index = i;

            //save non zero chance indices:
            if (piece.Chance > 0)
               nonZeroChancesIndices.Add(i);
         }
      }

      private void OnValidate()
      {
         if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
            Debug.LogError("[ SpinWheel ]  pieces length must be between " + piecesMin + " and " + piecesMax);
      }
   }
}