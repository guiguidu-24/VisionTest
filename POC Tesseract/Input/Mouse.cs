using WindowsInput.Events;
using WindowsInput;

namespace Core.Input
{

    public class Mouse : IMouse
    {
        private readonly IScreen _screen = new Screen();

        
        public void DoubleClick()
        {
            Simulate.Events().DoubleClick(ButtonCode.Left).Invoke().Wait();
        }

        public void LeftClick()
        {
            Simulate.Events().Click(ButtonCode.Left).Invoke().Wait();
        }

        public void LeftDown()
        {
            Simulate.Events().Hold(ButtonCode.Left).Invoke().Wait();
        }

        public void LeftUp()
        {
            Simulate.Events().Release(ButtonCode.Left).Invoke().Wait();
        }

        public void MoveBy(int deltaX, int deltaY)
        {
            Simulate.Events().MoveBy(CoordinateCorrection(deltaX), CoordinateCorrection(deltaY)).Invoke().Wait();
        }

        public void MoveTo(int x, int y)
        {
            Simulate.Events().MoveTo(CoordinateCorrection(x), CoordinateCorrection(y)).Invoke().Wait();
        }

        public void RightClick()
        {
            Simulate.Events().Click(ButtonCode.Right).Invoke().Wait();
        }

        public void RightDown()
        {
            Simulate.Events().Hold(ButtonCode.Right).Invoke().Wait();
        }

        public void RightUp()
        {
            Simulate.Events().Release(ButtonCode.Right).Invoke().Wait();
        }

        [Obsolete("⚠️ Not tested — use with caution.", false)]
        public void ScrollHorizontal(int delta) //TODO Test scrolling
        {
            Simulate.Events().Scroll(ButtonCode.None, ButtonScrollDirection.Right, delta).Invoke().Wait();
        }

        [Obsolete("⚠️ Not tested — use with caution.", false)]
        public void ScrollVertical(int delta)
        {
            Simulate.Events().Scroll( ButtonCode.None, ButtonScrollDirection.Up, delta).Invoke().Wait();
        }

        private int CoordinateCorrection(int coordinate)
        {
            return (int)(coordinate / _screen.ScaleFactor);
        }
    }
}