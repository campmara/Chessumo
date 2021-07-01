using UnityEngine;
using System.Collections.Generic;


namespace Mara.MrTween {
    /// <summary>
    /// useful enum for any transform related property tweens
    /// </summary>
    public enum TransformTargetType {
        Position,
        XPosition,
        YPosition,
        ZPosition,
        LocalPosition,
        XLocalPosition,
        YLocalPosition,
        ZLocalPosition,
        LocalScale,
        XLocalScale,
        YLocalScale,
        ZLocalScale,
        EulerAngles,
        LocalEulerAngles
    }

    /// <summary>
    /// this is a special case since Transforms are by far the most tweened object. we encapsulate the Tween and the ITweenTarget
    /// in a single, cacheable class
    /// </summary>
    public class TransformVector3Tween : Vector3Tween, ITweenTarget<Vector3> {
        Transform _transform;
        TransformTargetType _targetType;

        public void SetTweenedValue(Vector3 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && _transform == null) {
                return;
            }

            switch (_targetType) {
                case TransformTargetType.Position:
                    _transform.position = value;
                    break;
                case TransformTargetType.LocalPosition:
                    _transform.localPosition = value;
                    break;
                case TransformTargetType.LocalScale:
                    _transform.localScale = value;
                    break;
                case TransformTargetType.EulerAngles:
                    _transform.eulerAngles = value;
                    break;
                case TransformTargetType.LocalEulerAngles:
                    _transform.localEulerAngles = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public Vector3 GetTweenedValue() {
            switch (_targetType) {
                case TransformTargetType.Position:
                    return _transform.position;
                case TransformTargetType.LocalPosition:
                    return _transform.localPosition;
                case TransformTargetType.LocalScale:
                    return _transform.localScale;
                case TransformTargetType.EulerAngles:
                    return _transform.eulerAngles;
                case TransformTargetType.LocalEulerAngles:
                    return _transform.localEulerAngles;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public new object GetTargetObject() {
            return _transform;
        }


        public void SetTargetAndType(Transform transform, TransformTargetType targetType) {
            _transform = transform;
            _targetType = targetType;
        }


        protected override void UpdateValue() {
            // special case for non-relative angle lerps so that they take the shortest possible rotation
            if ((_targetType == TransformTargetType.EulerAngles || _targetType == TransformTargetType.LocalEulerAngles) && !_isRelative) {
                if (_animationCurve != null)
                    SetTweenedValue(TweenMath.EaseAngle(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
                else
                    SetTweenedValue(TweenMath.EaseAngle(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
            } else {
                if (_animationCurve != null)
                    SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
                else
                    SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
            }
        }


        public override void RecycleSelf() {
            if (_shouldRecycleTween) {
                _target = null;
                _nextTween = null;
                _transform = null;
                QuickCache<TransformVector3Tween>.Push(this);
            }
        }
    }

    public class TransformFloatTween : FloatTween, ITweenTarget<float> {
        Transform _transform;
        TransformTargetType _targetType;

        public void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_transform)
                return;

            switch (_targetType) {
                case TransformTargetType.XPosition:
                    _transform.position = new Vector3(value, _transform.position.y, _transform.position.z);
                    break;
                case TransformTargetType.YPosition:
                    _transform.position = new Vector3(_transform.position.x, value, _transform.position.z);
                    break;
                case TransformTargetType.ZPosition:
                    _transform.position = new Vector3(_transform.position.x, _transform.position.y, value);
                    break;

                case TransformTargetType.XLocalPosition:
                    _transform.localPosition = new Vector3(value, _transform.localPosition.y, _transform.localPosition.z);
                    break;
                case TransformTargetType.YLocalPosition:
                    _transform.localPosition = new Vector3(_transform.localPosition.x, value, _transform.localPosition.z);
                    break;
                case TransformTargetType.ZLocalPosition:
                    _transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, value);
                    break;

                case TransformTargetType.XLocalScale:
                    _transform.localScale = new Vector3(value, _transform.localScale.y, _transform.localScale.z);
                    break;
                case TransformTargetType.YLocalScale:
                    _transform.localScale = new Vector3(_transform.localScale.x, value, _transform.localScale.z);
                    break;
                case TransformTargetType.ZLocalScale:
                    _transform.localScale = new Vector3(_transform.localScale.x, _transform.localScale.y, value);
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public float GetTweenedValue() {
            switch (_targetType) {
                case TransformTargetType.XPosition:
                    return _transform.position.x;
                case TransformTargetType.YPosition:
                    return _transform.position.y;
                case TransformTargetType.ZPosition:
                    return _transform.position.z;

                case TransformTargetType.XLocalPosition:
                    return _transform.localPosition.x;
                case TransformTargetType.YLocalPosition:
                    return _transform.localPosition.y;
                case TransformTargetType.ZLocalPosition:
                    return _transform.localPosition.z;

                case TransformTargetType.XLocalScale:
                    return _transform.localScale.x;
                case TransformTargetType.YLocalScale:
                    return _transform.localScale.y;
                case TransformTargetType.ZLocalScale:
                    return _transform.localScale.z;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public new object GetTargetObject() {
            return _transform;
        }

        public void SetTargetAndType(Transform transform, TransformTargetType targetType) {
            _transform = transform;
            _targetType = targetType;
        }

        protected override void UpdateValue() {
            if (_animationCurve != null) {
                SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            } else {
                SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
            }
        }

        public override void RecycleSelf() {
            if (_shouldRecycleTween) {
                _target = null;
                _nextTween = null;
                _transform = null;
                QuickCache<TransformFloatTween>.Push(this);
            }
        }
    }
}