using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Image))]
    public class UI_Knob : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler
    {
        public enum Direction { CW, CCW };
        public Direction direction = Direction.CW;
        [HideInInspector]
        public float knobValue;
        public float maxValue = 0;
        public bool snapToPosition = false;
        [Space(30)]
        public KnobFloatValueEvent OnValueChanged;
        private float _previousValue = 0;
        private float _initAngle;
        private float _currentAngle;
        private Vector2 _currentVector;
        private Quaternion _initRotation;
        private bool _canDrag = false;

        //ONLY ALLOW ROTATION WITH POINTER OVER THE CONTROL
        public void OnPointerDown(PointerEventData eventData)
        {
            _canDrag = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _canDrag = false;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _canDrag = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _canDrag = false;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            SetInitPointerData(eventData);
        }
        void SetInitPointerData(PointerEventData eventData)
        {
            _initRotation = transform.rotation;
            _currentVector = eventData.position - (Vector2)transform.position;
            _initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;
        }
        public void OnDrag(PointerEventData eventData)
        {
            //CHECK IF CAN DRAG
            if (!_canDrag)
            {
                SetInitPointerData(eventData);
                return;
            }
            _currentVector = eventData.position - (Vector2)transform.position;
            _currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;

            Quaternion addRotation = Quaternion.AngleAxis(_currentAngle - _initAngle, this.transform.forward);
            addRotation.eulerAngles = new Vector3(0, 0, addRotation.eulerAngles.z);

            Quaternion finalRotation = _initRotation * addRotation;

            if (direction == Direction.CW)
            {
                knobValue = 1 - (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 - 360 * knobValue);
                }
            }
            else
            {
                knobValue = (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 * knobValue);
                }
            }

            //CHECK MAX VALUE
            if (maxValue > 0)
            {
                if (knobValue > maxValue)
                {
                    knobValue = maxValue;
                    float maxAngle = direction == Direction.CW ? 360f - 360f * maxValue : 360f * maxValue;
                    transform.localEulerAngles = new Vector3(0, 0, maxAngle);
                    SetInitPointerData(eventData);
                    InvokeEvents(knobValue);
                    return;
                }
            }

            transform.rotation = finalRotation;
            InvokeEvents(knobValue);

            _previousValue = knobValue;
        }
        private void SnapToPosition(ref float knobValue)
        {
            float snapStep = 1 / (float)(1);
            float newValue = Mathf.Round(knobValue / snapStep) * snapStep;
            knobValue = newValue;
        }
        private void InvokeEvents(float value)
        {
            OnValueChanged.Invoke(value);
        }
    }

    [System.Serializable]
    public class KnobFloatValueEvent : UnityEvent<float> { }

}