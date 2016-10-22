using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CodeCreators.UWP.UI
{
  public static class Creators
  {

    #region Constants 

    static string PlaneProjectPropertyRotationX = "(UIElement.Projection).(PlaneProjection.RotationX)";
    static string PlaneProjectPropertyRotationY = "(UIElement.Projection).(PlaneProjection.RotationY)";
    static string RenderTransformScaleX = "(UIElement.RenderTransform).(CompositeTransform.ScaleX)";
    static string RenderTransformScaleY = "(UIElement.RenderTransform).(CompositeTransform.ScaleY)";
    static string RenderTransformRotation = "(UIElement.RenderTransform).(CompositeTransform.Rotation)";
    static string RenderTransformTranslateX = "(UIElement.RenderTransform).(CompositeTransform.TranslateX)";
    static string RenderTransformTranslateY = "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";
    static string OpacityProperty = "Opacity";

    #endregion Constants 


    /// <summary>
    /// Changes the opacity for the specified Framework Element.
    /// </summary>
    /// <param name="element">Framework Element to be faded.</param>
    /// <param name="from">Opacity Starting point.</param>
    /// <param name="to">Opacity Ending point.</param>
    /// <param name="duration">Length of time in Milliseconds that the storyboard will take to fade the opacity.</param>
    /// <param name="begin">Begin time to start the opacity transformation in milliseconds.</param>
    /// <returns>Storyboard</returns>
    public static Storyboard CreateElementOpacityFadeStoryBoard(FrameworkElement element, double from, double to, double duration = 300d, double begin = 0)
    {
      Storyboard storyboard = new Storyboard();
      if (element == null) return storyboard;

      try
      {
        DoubleAnimation doubleAnimationOpacity = new DoubleAnimation();
        doubleAnimationOpacity.From = from;
        doubleAnimationOpacity.To = to;
        doubleAnimationOpacity.BeginTime = TimeSpan.FromMilliseconds(begin);
        doubleAnimationOpacity.Duration = TimeSpan.FromMilliseconds(duration);

        CubicEase easing = new CubicEase();
        if (from > to) easing.EasingMode = EasingMode.EaseOut; //scale in
        else easing.EasingMode = EasingMode.EaseOut;//probably scale out
        doubleAnimationOpacity.EasingFunction = easing;

        Storyboard.SetTargetProperty(doubleAnimationOpacity, OpacityProperty);
        Storyboard.SetTarget(doubleAnimationOpacity, element);

        storyboard.Children.Add(doubleAnimationOpacity);
      }
      catch (Exception ex)
      {
      }

      return storyboard;
    }




    /// <summary>
    /// Creates an animation that spirals the element either up or down in scale.
    /// </summary>
    /// <param name="element">Framework Element to be scaled and rotated.</param>
    /// <param name="fromX">Scaling X Starting point.</param>
    /// <param name="fromY">Scaling Y Starting point.</param>
    /// <param name="toX">Scaling X Ending point.</param>
    /// <param name="toY">Scaling Y Ending point.</param>
    /// <param name="durationX">Length of time in Milliseconds that the storyboard will take to scale on the X axis.</param>
    /// <param name="durationY">Length of time in Milliseconds that the storyboard will take to scale on the Y axis.</param>
    /// <param name="durationRotate">Length of time in Milliseconds that the storyboard will take to perform the number of rotations indicated.</param>
    /// <param name="beginX">Begin time to start the scale X transformation in milliseconds.</param>
    /// <param name="beginY">Begin time to start the scale Y transformation in milliseconds.</param>
    /// <param name="beginRotate">Begin time to start the rotation</param>
    /// <param name="rotations">Number of rotations to perform.</param>
    /// <returns>Storyboard</returns>
    public static Storyboard CreateSpiralStoryboard(FrameworkElement element, double fromX, double fromY, double toX, double toY, double durationX = 300d, double durationY = 300d, double durationRotate = 300d, double beginX = 0, double beginY = 0, double beginRotate = 0, double rotations = 1)
    {
      Storyboard storyboard = new Storyboard();
      if (element == null) return storyboard;

      try
      {
        DoubleAnimation doubleAnimationScaleX = new DoubleAnimation();
        doubleAnimationScaleX.From = fromX;
        doubleAnimationScaleX.To = toX;
        doubleAnimationScaleX.BeginTime = TimeSpan.FromMilliseconds(beginX);
        doubleAnimationScaleX.Duration = TimeSpan.FromMilliseconds(durationX);

        DoubleAnimation doubleAnimationScaleY = new DoubleAnimation();
        doubleAnimationScaleY.From = fromY;
        doubleAnimationScaleY.To = toY;
        doubleAnimationScaleY.BeginTime = TimeSpan.FromMilliseconds(beginY);
        doubleAnimationScaleY.Duration = TimeSpan.FromMilliseconds(durationY);

        DoubleAnimation doubleAnimationRotate = new DoubleAnimation();
        doubleAnimationRotate.From = 0;
        doubleAnimationRotate.To = 360d * rotations;
        doubleAnimationRotate.BeginTime = TimeSpan.FromMilliseconds(beginRotate);
        doubleAnimationRotate.Duration = TimeSpan.FromMilliseconds(durationRotate);

        CubicEase scaleEasing = new CubicEase();
        if (fromX + fromY > toX + toY) scaleEasing.EasingMode = EasingMode.EaseOut; //scale in
        else scaleEasing.EasingMode = EasingMode.EaseOut;//probably scale out
        doubleAnimationScaleX.EasingFunction = scaleEasing;
        doubleAnimationScaleY.EasingFunction = scaleEasing;
        doubleAnimationRotate.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut };

        Storyboard.SetTargetProperty(doubleAnimationScaleX, RenderTransformScaleX);
        Storyboard.SetTarget(doubleAnimationScaleX, element);
        Storyboard.SetTargetProperty(doubleAnimationScaleY, RenderTransformScaleY);
        Storyboard.SetTarget(doubleAnimationScaleY, element);
        Storyboard.SetTargetProperty(doubleAnimationRotate, RenderTransformRotation);
        Storyboard.SetTarget(doubleAnimationRotate, element);

        storyboard.Children.Add(doubleAnimationScaleX);
        storyboard.Children.Add(doubleAnimationScaleY);
        storyboard.Children.Add(doubleAnimationRotate);
      }
      catch (Exception ex)
      {
      }
      return storyboard;
    }

    /// <summary>
    /// Creates an animation that spirals the element either up or down in scale.
    /// </summary>
    /// <param name="element">Framework Element to be scaled and rotated.</param>
    /// <param name="from">Scaling Starting point.</param>
    /// <param name="to">Scaling Ending point.</param>
    /// <param name="duration">Length of time in Milliseconds that the storyboard will run for.</param>
    /// <param name="rotations">Number of rotations to perform.</param>
    /// <returns>Storyboard</returns>
    public static Storyboard CreateSpiralStoryboard(FrameworkElement element, double from, double to, double duration = 300d, double rotations = 1)
    {
      return CreateSpiralStoryboard(element, from, from, to, to, duration, duration, duration, 0, 0, 0, rotations);
    }


    /// <summary>
    /// Creates a Scaling animation for the submitted Framework Element object.
    /// </summary>
    /// <param name="element">Framework Element to be scaled.</param>
    /// <param name="fromX">Scaling X Starting point.</param>
    /// <param name="fromY">Scaling Y Starting point.</param>
    /// <param name="toX">Scaling X Ending point.</param>
    /// <param name="toY">Scaling Y Ending point.</param>
    /// <param name="durationX">Length of time in Milliseconds that the storyboard will take to scale on the X axis.</param>
    /// <param name="durationY">Length of time in Milliseconds that the storyboard will take to scale on the Y axis.</param>
    /// <param name="beginX">Begin time to start the scale X transformation in milliseconds.</param>
    /// <param name="beginY">Begin time to start the scale Y transformation in milliseconds.</param>
    /// <returns>Storyboard</returns>
    public static Storyboard CreateScalingStoryBoard(FrameworkElement element, double fromX, double fromY, double toX, double toY, double durationX = 300d, double durationY = 300d, double beginX = 0, double beginY = 0)
    {
      Storyboard storyboard = new Storyboard();
      if (element == null) return storyboard;

      try
      {
        DoubleAnimation doubleAnimationScaleX = new DoubleAnimation();
        doubleAnimationScaleX.From = fromX;
        doubleAnimationScaleX.To = toX;
        doubleAnimationScaleX.BeginTime = TimeSpan.FromMilliseconds(beginX);
        doubleAnimationScaleX.Duration = TimeSpan.FromMilliseconds(durationX);

        DoubleAnimation doubleAnimationScaleY = new DoubleAnimation();
        doubleAnimationScaleY.From = fromY;
        doubleAnimationScaleY.To = toY;
        doubleAnimationScaleY.BeginTime = TimeSpan.FromMilliseconds(beginY);
        doubleAnimationScaleY.Duration = TimeSpan.FromMilliseconds(durationY);

        CubicEase easing = new CubicEase();
        if (fromX + fromY > toX + toY) easing.EasingMode = EasingMode.EaseOut; //scale in
        else easing.EasingMode = EasingMode.EaseOut;//probably scale out
        doubleAnimationScaleX.EasingFunction = easing;
        doubleAnimationScaleY.EasingFunction = easing;

        Storyboard.SetTargetProperty(doubleAnimationScaleX, RenderTransformScaleX);
        Storyboard.SetTarget(doubleAnimationScaleX, element);
        Storyboard.SetTargetProperty(doubleAnimationScaleY, RenderTransformScaleY);
        Storyboard.SetTarget(doubleAnimationScaleY, element);

        storyboard.Children.Add(doubleAnimationScaleX);
        storyboard.Children.Add(doubleAnimationScaleY);
      }
      catch (Exception ex)
      {
      }
      return storyboard;
    }

    /// <summary>
    /// Creates a Scaling animation for the submitted Framework Element object.
    /// </summary>
    /// <param name="element">Framework Element to be scaled.</param>
    /// <param name="from">Scaling Starting point.</param>
    /// <param name="to">Scaling Ending point.</param>
    /// <param name="duration">Length of time in Milliseconds that the storyboard will run for.</param>
    /// <returns>Storyboard.</returns>
    public static Storyboard CreateScalingStoryBoard(FrameworkElement element, double from, double to, double duration = 300d)
    {
      return CreateScalingStoryBoard(element, from, from, to, to, duration, duration);
    }


    /// <summary>
    /// Creates a Pointer Down Storyboard with custom static animation
    /// on the specified UIElement.
    /// </summary>
    /// <param name="element">Framework Element to be animated.</param>
    /// <returns>Storyboard.</returns>
    public static Storyboard PointerDownStory(UIElement uiElement)
    {
      Storyboard storyboard = new Storyboard();

      try
      {
        uiElement.RenderTransform = new CompositeTransform();

        DoubleAnimation doubleAnimationX = new DoubleAnimation();
        DoubleAnimation doubleAnimationY = new DoubleAnimation();

        doubleAnimationX.AutoReverse = false;
        doubleAnimationX.AutoReverse = false;
        doubleAnimationX.Duration = TimeSpan.FromMilliseconds(100);
        doubleAnimationY.Duration = TimeSpan.FromMilliseconds(100);
        doubleAnimationX.To = 0.85;
        doubleAnimationY.To = 0.85;
        storyboard.Children.Add(doubleAnimationX);
        storyboard.Children.Add(doubleAnimationY);

        Storyboard.SetTarget(doubleAnimationX, uiElement);
        Storyboard.SetTarget(doubleAnimationY, uiElement);
        Storyboard.SetTargetProperty(doubleAnimationX, RenderTransformScaleX);
        Storyboard.SetTargetProperty(doubleAnimationY, RenderTransformScaleY);

      }
      catch (Exception ex)
      {
      }
      return storyboard;
    }

    /// <summary>
    /// Creates a Pointer Up Storyboard with custom static animation
    /// on the specified UIElement.
    /// </summary>
    /// <param name="element">Framework Element to be animated.</param>
    /// <returns>Storyboard.</returns>
    public static Storyboard PointerUpStory(UIElement uiElement)
    {

      Storyboard storyboard = new Storyboard();

      try
      {
        uiElement.RenderTransform = new CompositeTransform();
        DoubleAnimation doubleAnimationX = new DoubleAnimation();
        DoubleAnimation doubleAnimationY = new DoubleAnimation();

        doubleAnimationX.AutoReverse = false;
        doubleAnimationX.AutoReverse = false;
        doubleAnimationX.Duration = TimeSpan.FromMilliseconds(100);
        doubleAnimationY.Duration = TimeSpan.FromMilliseconds(100);
        doubleAnimationX.To = 1.00;
        doubleAnimationY.To = 1.00;

        storyboard.Children.Add(doubleAnimationX);
        storyboard.Children.Add(doubleAnimationY);

        Storyboard.SetTarget(doubleAnimationX, uiElement);
        Storyboard.SetTarget(doubleAnimationY, uiElement);
        Storyboard.SetTargetProperty(doubleAnimationX, RenderTransformScaleX);
        Storyboard.SetTargetProperty(doubleAnimationY, RenderTransformScaleY);

      }
      catch (Exception ex)
      {
      }
      return storyboard;
    }


    public static void PerformClickAnimation(UIElement uiElement)
    {
      Storyboard storyboard = Creators.PointerDownStory(uiElement as UIElement);
      storyboard.Completed += (sbs, sbe) =>
      {
        Storyboard storyBoardEnd = Creators.PointerUpStory(uiElement as UIElement);
        storyBoardEnd.BeginTime = TimeSpan.FromMilliseconds(200);
        storyBoardEnd.Begin();
      };
      storyboard.Begin();
    }

    /// <summary>
    /// Creates a PointerUpThemeAnimation for the
    /// specified Grid control.
    /// </summary>
    /// <param name="grid">Grid to perform the animation on.</param>
    /// <returns>Storyboard.</returns>
    public static Storyboard PointerUpStoryboard(Grid grid)
    {
      Storyboard storyboard = new Storyboard();
      PointerUpThemeAnimation pointerUpThemeAnimation = new PointerUpThemeAnimation();
      Storyboard.SetTarget(pointerUpThemeAnimation, grid);

      return storyboard;
    }

    /// <summary>
    /// Creates a PointerDownThemeAnimation for the
    /// specified Grid control.
    /// </summary>
    /// <param name="grid">Grid to perform the animation on.</param>
    /// <returns>Storyboard.</returns>
    public static Storyboard PointerDownStoryboard(Grid grid)
    {
      Storyboard storyboard = new Storyboard();
      PointerDownThemeAnimation pointerDownThemeAnimation = new PointerDownThemeAnimation();
      Storyboard.SetTarget(pointerDownThemeAnimation, grid);

      return storyboard;
    }

    /// <summary>
    /// Properly handles Pointer Pressed Event
    /// of a Grid element to properly execute
    /// a Pointer Down Theme Storyboard Animation.
    /// </summary>
    /// <param name="grid">Grid to perform the animation on.</param>
    public static void PointerPressedHandler(Grid grid)
    {
      grid.Projection = new PlaneProjection();
      PointerDownStoryboard(grid).Begin();
    }

    /// <summary>
    /// Properly handles Pointer Pressed Event
    /// of a Grid element to properly execute
    /// a Pointer Up Theme Storyboard Animation.
    /// </summary>
    /// <param name="grid">Grid to perform the animation on.</param>
    public static void PointerReleasedHandler(Grid grid)
    {
      PointerUpStoryboard(grid).Begin();
    }

    /// <summary>
    /// Moves a specified Element on a relatively flat plane from 
    /// one defined set of coordinates to another defined set of 
    /// coordinates in the time specified as duration.
    /// 
    /// This implementation only allows for linear diagonal travel
    /// </summary>
    /// <param name="element">FrameworkElement to be moved.</param>
    /// <param name="from">Point defining X and Y cordinates of starting position.</param>
    /// <param name="to">Point defining X and Y cordinates of ending position.</param>
    /// <param name="duration">
    /// Duration as a double defining the time alloted. This will cause the 
    /// FrameworkElement to move in a linear diagional direction over  
    /// constant time duration to the ending position.
    /// </param>
    /// <returns>Storyboard</returns>
    public static Storyboard MoveElementStoryboard(FrameworkElement element, Point from, Point to, double duration)
    {
      return MoveElementStoryboard(element, from.X, from.Y, to.X, to.Y, duration, duration);
    }

    /// <summary>
    /// Moves a specified Element on a relatively flat plane from 
    /// one defined set of coordinates to another defined set of 
    /// coordinates in the time specified as duration.
    /// 
    /// This implementation allows for the travel along the X and 
    /// Y axis to occur at different relative speeds.
    /// </summary>
    /// <param name="element">FrameworkElement to be moved.</param>
    /// <param name="from">Point defining X and Y cordinates of starting position.</param>
    /// <param name="to">Point defining X and Y cordinates of ending position.</param>
    /// <param name="duration">
    /// Point value containing the duration for the X travel and the Y travel durations.
    /// This will allow the FrameworkElement to arrive at one ending coordiate 
    /// independant of the time taken to arrive at the other ending coordiate.
    /// </param>
    /// <returns>Storyboard</returns>
    public static Storyboard MoveElementStoryboard(FrameworkElement element, Point from, Point to, Point duration)
    {
      return MoveElementStoryboard(element, from.X, from.Y, to.X, to.Y, duration.X, duration.Y);
    }

    /// <summary>
    /// Moves a specified Element on a relatively flat plane from 
    /// one defined set of coordinates to another defined set of 
    /// coordinates in the time specified as duration.
    /// 
    /// This implementation allows for the travel along the X and 
    /// Y axis to occur at different relative speeds.
    /// </summary>
    /// <param name="element">FrameworkElement to be moved.</param>
    /// <param name="fromX">Double defining X cordinate of starting position.</param>
    /// <param name="fromY">Double defining Y cordinate of starting position.</param>
    /// <param name="toX">Double defining X cordinate of ending position.</param>
    /// <param name="toY">Double defining Y cordinate of ending position.</param>
    /// <param name="durationX">Double defining the duration of travel to the X cordinate of the ending position.</param>
    /// <param name="durationY">Double defining the duration of travel to the Y cordinate of the ending position.</param>
    /// <returns>Storyboard</returns>
    public static Storyboard MoveElementStoryboard(FrameworkElement element, double fromX, double fromY, double toX, double toY, double durationX, double durationY)
    {
      Storyboard storyboard = new Storyboard();

      try
      {
        DoubleAnimation doubleAnimationTranslateX = new DoubleAnimation();
        doubleAnimationTranslateX.From = fromX;
        doubleAnimationTranslateX.To = toX;
        doubleAnimationTranslateX.Duration = TimeSpan.FromMilliseconds(durationX);

        DoubleAnimation doubleAnimationTranslateY = new DoubleAnimation();
        doubleAnimationTranslateY.From = fromY;
        doubleAnimationTranslateY.To = toY;
        doubleAnimationTranslateY.Duration = TimeSpan.FromMilliseconds(durationY);

        CircleEase easing = new CircleEase() { EasingMode = EasingMode.EaseOut };

        doubleAnimationTranslateX.EasingFunction = easing;
        doubleAnimationTranslateY.EasingFunction = easing;

        Storyboard.SetTargetProperty(doubleAnimationTranslateX, RenderTransformTranslateX);
        Storyboard.SetTarget(doubleAnimationTranslateX, element);
        Storyboard.SetTargetProperty(doubleAnimationTranslateY, RenderTransformTranslateY);
        Storyboard.SetTarget(doubleAnimationTranslateY, element);

        storyboard.Children.Add(doubleAnimationTranslateX);
        storyboard.Children.Add(doubleAnimationTranslateY);

      }
      catch (Exception ex)
      {
      }
      return storyboard;
    }

    public enum FlipAxis { FlipOnXAxis, FlipOnYAxis };

    /// <summary>
    /// Rotates an element x number of times in based on submitted repeat times.
    /// 
    /// 
    /// <Usage>
    /// 
    /// FlipOnAxisStoryboard(myGrid, 0, -180, FlipAxis.FlipOnXAxis, 3, 2);
    /// 
    /// </Usage>
    /// </summary>
    /// <param name="element">FrameworkElement to be moved.</param>
    /// <param name="from">Double defining starting position.</param>
    /// <param name="to">Double defining ending position.</param>
    /// <param name="flipAxis">Enumerated value indicating axis to perform the rotation on.</param>
    /// <param name="duration">Double defining the duration of rotation to the ending position</param>
    /// <param name="repeatTimes"></param>
    /// <returns></returns>
    public static Storyboard FlipOnAxisStoryboard(FrameworkElement element, double from, double to, FlipAxis flipAxis, double duration = 3, double repeatTimes = 1)
    {
      if (element.Projection == null) element.Projection = new PlaneProjection();
      else if (element.Projection.GetType() != typeof(PlaneProjection)) element.Projection = new PlaneProjection();


      Storyboard storyboard = new Storyboard();

      DoubleAnimation doubleAnimationRotate = new DoubleAnimation();
      doubleAnimationRotate.From = from;
      doubleAnimationRotate.To = to * repeatTimes;
      doubleAnimationRotate.Duration = TimeSpan.FromMilliseconds(duration);

      switch (flipAxis)
      {
        case FlipAxis.FlipOnXAxis:
          Storyboard.SetTargetProperty(doubleAnimationRotate, PlaneProjectPropertyRotationX);
          break;
        case FlipAxis.FlipOnYAxis:
          Storyboard.SetTargetProperty(doubleAnimationRotate, PlaneProjectPropertyRotationY);
          break;
        default:
          Storyboard.SetTargetProperty(doubleAnimationRotate, PlaneProjectPropertyRotationX);
          break;
      }

      Storyboard.SetTarget(doubleAnimationRotate, element);

      return storyboard;
    }


  }


}

