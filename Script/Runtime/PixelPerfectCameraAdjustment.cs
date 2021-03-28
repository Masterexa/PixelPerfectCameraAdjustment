using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

namespace Kimiguna.Graphics2D{
    [RequireComponent(typeof(UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera))]
    public class PixelPerfectCameraAdjustment : MonoBehaviour
    {
        public enum MatchMode
        {
            WidthOrHeight,
            Trim,
            Extend,
            Envelove,
        }

        #region Fields
            [FormerlySerializedAs("m_referenceResolution")]
            [SerializeField]Vector2Int m_baseResolution = new Vector2Int(400,300);
            public Vector2Int baseResolution
            {
                get => m_baseResolution;
                set => m_baseResolution = value;
            }

            [System.Obsolete]
            public Vector2Int referenceResolution
            {
                get => m_baseResolution;
                set => m_baseResolution = value;
            }


            [Space]
            [SerializeField]MatchMode m_matchMode = MatchMode.Extend;
            public MatchMode matchMode
            {
                get => m_matchMode;
                set => m_matchMode = value;
            }

            [HideInInspector,Range(0,1)]
            [SerializeField]float m_matchRatio = 1f;
            public float matchRatio
            {
                get => m_matchRatio;
                set => m_matchRatio = Mathf.Clamp01(value);
            }

            UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera  m_pixelPerfectCamera;
            Camera  m_camera;

            public float computedMatchRatio{get; private set;}
        #endregion

        #region Events
            void Awake()
            {
                m_pixelPerfectCamera = GetComponent<UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera>();
                m_camera = GetComponent<Camera>();
            }

            void LateUpdate()
            {
                var res = m_baseResolution;
                var refAspect = (float)res.x/(float)res.y;
                var camAspect = m_camera.aspect;
                var reciprocalAspect = 1f/camAspect;

                computedMatchRatio =
                    (matchMode==MatchMode.Trim) ?  ( (refAspect<camAspect) ? 0f : 1f ):
                    (matchMode==MatchMode.Extend) ?  ( (refAspect>camAspect) ? 0f : 1f ):
                    (matchMode==MatchMode.Envelove) ?  Mathf.Clamp01( Mathf.Max(camAspect,reciprocalAspect) ):
                    m_matchRatio
                ;

                m_pixelPerfectCamera.refResolutionX = (int)Mathf.Lerp(res.x, (float)res.y*camAspect, computedMatchRatio);
                m_pixelPerfectCamera.refResolutionY = (int)Mathf.Lerp((float)res.x*reciprocalAspect, res.y, computedMatchRatio);

                // Crop to less than half the resolution of the camera
                
                m_pixelPerfectCamera.refResolutionX = Mathf.Min(m_pixelPerfectCamera.refResolutionX, m_camera.pixelWidth)/2*2;
                m_pixelPerfectCamera.refResolutionY = Mathf.Min(m_pixelPerfectCamera.refResolutionY, m_camera.pixelHeight)/2*2;
            }
        #endregion
    }
}