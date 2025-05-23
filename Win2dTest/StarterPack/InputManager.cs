using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2dTest.StarterPack
{
    public class InputManager
    {
        private readonly ConcurrentQueue<InputAction> _actionQueue = [];
        private readonly CanvasAnimatedControl _canvas;

        private PointerRoutedEventArgs? _lastPointerArgs;

        public InputManager(CanvasAnimatedControl canvas)
        {
            _canvas = canvas;

            canvas.PointerReleased += PointerReleased;
            canvas.PointerMoved += PointerMoved;
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _actionQueue.Enqueue(new InputAction(_canvas, InputActionType.PointerReleased, e));
        }

        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _actionQueue.Enqueue(new InputAction(_canvas, InputActionType.PointerMoved, e, _lastPointerArgs));
        }

        public IReadOnlyList<InputAction> DequeueInputs()
        {
            List<InputAction> dequeuedActions = [];

            while (!_actionQueue.IsEmpty)
            {
                if (_actionQueue.TryDequeue(out InputAction? dequeuedAction))
                {
                    dequeuedActions.Add(dequeuedAction);
                }
            }

            return dequeuedActions.AsReadOnly();
        }
    }

    public class InputAction
    {
        public InputAction(CanvasAnimatedControl canvas, InputActionType actionType, PointerRoutedEventArgs e)
        {
            Type = actionType;
            PointerReleased = new PointerReleasedActionInfo(canvas, e);
        }

        public InputAction(CanvasAnimatedControl canvas, InputActionType actionType, PointerRoutedEventArgs e, PointerRoutedEventArgs? lastPointerArgs)
        {
            Type = InputActionType.PointerMoved;
            PointerMoved = new PointerMovedActionInfo(canvas, e, lastPointerArgs);
        }

        public InputActionType Type { get; }

        public PointerReleasedActionInfo? PointerReleased { get; }

        public PointerMovedActionInfo? PointerMoved { get; }
    }

    public class PointerReleasedActionInfo
    {
        public PointerReleasedActionInfo(CanvasAnimatedControl canvas, PointerRoutedEventArgs args)
        {
            RawArgs = args;

           PointerPoint pointerPoint = args.GetCurrentPoint(canvas);
           Position = new Vector2((float)pointerPoint.Position.X, (float)pointerPoint.Position.Y);
           Properties = pointerPoint.Properties;
        }

        public PointerRoutedEventArgs RawArgs { get; }

        public Vector2 Position { get; }

        public PointerPointProperties Properties { get; }
    }

    public class PointerMovedActionInfo
    {
        public PointerMovedActionInfo(CanvasAnimatedControl canvas, PointerRoutedEventArgs args, PointerRoutedEventArgs? lastPointerArgs)
        {
            RawArgs = args;

            PointerPoint pointerPoint = args.GetCurrentPoint(canvas);
            Position = new Vector2((float)pointerPoint.Position.X, (float)pointerPoint.Position.Y);
            Properties = pointerPoint.Properties;

            if (lastPointerArgs is not null)
            {
                PointerPoint lastPointerPoint = lastPointerArgs.GetCurrentPoint(canvas);
                MoveDelta = Position - new Vector2((float)pointerPoint.Position.X, (float)pointerPoint.Position.Y);
            }
        }

        public PointerRoutedEventArgs RawArgs { get; }

        public Vector2 Position { get; }

        public Vector2 MoveDelta { get; }

        public PointerPointProperties Properties { get; }
    }

    public enum InputActionType
    {
        PointerReleased,
        PointerMoved
    }
}
