namespace tests;

using System.ComponentModel;
using DarkAgesRPG.GUI;
using Xunit.Sdk;

public class GuiTest
{

    [Fact]
    public void TestRootWidgetPosition()
    {
        Widget RootWidget = new();
        Vector2i position = new Vector2i(100, 100);

        RootWidget.RelativePosition = position;

        Assert.True(RootWidget.Position == position);
    }

    [Fact]
    public void TestWidgetParents()
    {
        Widget RootWidget = new();
        Vector2i position = new Vector2i(100, 100);

        RootWidget.RelativePosition = position;

        Widget parent1 = new Widget();
        parent1.styles.Set("PaddingDirection", paddingDirecton.horizontal);
        RootWidget.PushWidget(parent1);

        Assert.True(parent1.Parent == RootWidget);

        Widget parent2 = new Widget();
        parent2.styles.Set("PaddingDirection", paddingDirecton.vertical);
        RootWidget.PushWidget(parent2);

        Assert.True(parent2.Parent == RootWidget);
    }

    [Fact]
    public void TestWidgetVerticalPadding(){
        Widget VerticalContainer = new();
        VerticalContainer.RelativePosition = new Vector2i(100, 100);
        VerticalContainer.styles.Set("PaddingIncrease", 10);

        Widget rectangle = new Widget();
        rectangle.Size = new Vector2i(100,100);

        Widget rectangle2 = new Widget();
        rectangle2.Size = new Vector2i(50,50);

        Widget rectangle3 = new Widget();
        rectangle3.Size = new Vector2i(150,150);

        VerticalContainer.PushWidget(rectangle);
        Assert.True(VerticalContainer.Size.X == 100);
        Assert.True(VerticalContainer.Size.Y == 110);
        Assert.True(VerticalContainer.paddingOffset.X == 0);
        Assert.True(VerticalContainer.paddingOffset.Y == 110);

        VerticalContainer.PushWidget(rectangle2);
        Assert.True(VerticalContainer.Size.X == 100);
        Assert.True(VerticalContainer.Size.Y == 170);
        Assert.True(VerticalContainer.paddingOffset.X == 0);
        Assert.True(VerticalContainer.paddingOffset.Y == 170);

        VerticalContainer.PushWidget(rectangle3);
        Assert.True(VerticalContainer.Size.X == 150);
        Assert.True(VerticalContainer.Size.Y == 330);
        Assert.True(VerticalContainer.paddingOffset.X == 0);
        Assert.True(VerticalContainer.paddingOffset.Y == 330);
    }

    [Fact]
    public void TestRemoveFinalPaddingHorizontal(){
        Widget HorizontalContainer = new();
        HorizontalContainer.styles.Set("PaddingIncrease",10);
        HorizontalContainer.styles.Set("PaddingDirection", paddingDirecton.horizontal);

        Widget item1 = new(){
            Size = new Vector2i(50,50)
        };

        Widget item2 = new(){
            Size = new Vector2i(50,50)
        };

        HorizontalContainer.PushWidget(item1);
        HorizontalContainer.PushWidget(item2);

        HorizontalContainer.RemovePadding();

        Assert.True(
            HorizontalContainer.paddingOffset == new Vector2i(50 + 10 + 50, 0)
        );
    }

    
    [Fact]
    public void TestRemoveFinalPaddingVertical(){
        Widget VerticalContainer = new();
        VerticalContainer.styles.Set("PaddingIncrease", 10);
        VerticalContainer.styles.Set("PaddingDirection", paddingDirecton.vertical);

        Widget item1 = new(){
            Size = new Vector2i(50,50)
        };

        Widget item2 = new(){
            Size = new Vector2i(50,50)
        };

        VerticalContainer.PushWidget(item1);
        VerticalContainer.PushWidget(item2);

        VerticalContainer.RemovePadding();

        Assert.True(
            VerticalContainer.paddingOffset == new Vector2i(0, 50 + 10 + 50)
        );
    }

    [Fact]
    public void TestWidgetHorizontalPadding(){
        Widget HorizontalContainer = new();
        HorizontalContainer.RelativePosition = new Vector2i(100, 100);
        HorizontalContainer.styles.Set("PaddingIncrease",10);
        HorizontalContainer.styles.Set("PaddingDirection", paddingDirecton.horizontal);

        Widget rectangle = new Widget();
        rectangle.Size = new Vector2i(100,100);

        Widget rectangle2 = new Widget();
        rectangle2.Size = new Vector2i(50,50);

        Widget rectangle3 = new Widget();
        rectangle3.Size = new Vector2i(150,150);

        HorizontalContainer.PushWidget(rectangle);
        Assert.True(HorizontalContainer.Size.X == 110);
        Assert.True(HorizontalContainer.Size.Y == 100);
        Assert.True(HorizontalContainer.paddingOffset.X == 110);
        Assert.True(HorizontalContainer.paddingOffset.Y == 0);
        Assert.True(rectangle.RelativePosition == new Vector2i(0,0));
        Assert.True(rectangle.Position == new Vector2i(100,100));

        HorizontalContainer.PushWidget(rectangle2);
        Assert.True(HorizontalContainer.Size.X == 170);
        Assert.True(HorizontalContainer.Size.Y == 100);
        Assert.True(HorizontalContainer.paddingOffset.X == 170);
        Assert.True(HorizontalContainer.paddingOffset.Y == 0);
        Assert.True(rectangle2.RelativePosition == new Vector2i(100 + 10 , 0));
        Assert.True(rectangle2.Position == new Vector2i(100 + 100 + 10, 100));

        HorizontalContainer.PushWidget(rectangle3);
        Assert.True(HorizontalContainer.Size.X == 330);
        Assert.True(HorizontalContainer.Size.Y == 150);
        Assert.True(HorizontalContainer.paddingOffset.X == 330);
        Assert.True(HorizontalContainer.paddingOffset.Y == 0);
        Assert.True(rectangle3.RelativePosition == new Vector2i(100 + 10 + 50 + 10, 0));
        Assert.True(rectangle3.Position == new Vector2i(100 + 100 + 10 + 50 + 10, 100));
    }


    /// <summary>
    /// Tests an horizontal root widget with 2 vertical containers with 2 items inside
    /// </summary>
    [Fact]
    public void TestNestedWidgetPadding(){
        Widget RootWidget = new();
        Vector2i rootPosition = new Vector2i(100, 100);

        RootWidget.RelativePosition = rootPosition;
        RootWidget.styles.Set("PaddingIncrease", 20);
        RootWidget.styles.Set("PaddingDirection", paddingDirecton.horizontal);

        Widget VerticalContainer = new();
        VerticalContainer.styles.Set("PaddingDirection", paddingDirecton.vertical);
        VerticalContainer.styles.Set("PaddingIncrease", 10);

        Widget Item0 = new(){
            Size = new Vector2i(100,100)
        };

        Widget Item1 = new(){
            Size = new Vector2i(150, 150)
        };


        Widget VerticalContainer2 = new();
        VerticalContainer2.styles.Set("PaddingDirection", paddingDirecton.vertical);
        VerticalContainer2.styles.Set("PaddingIncrease", 10);

        Widget Item2 = new(){
            Size = new Vector2i(150,150)
        };
        Widget Item3 = new(){
            Size = new Vector2i(100,100)
        };

        /// Assert container 1

        {
            // assert item 0 position
            Vector2i item0Position = VerticalContainer.GetNextWidgetPosition();
            Assert.True(item0Position == new Vector2i(0,0));
            VerticalContainer.PushWidget(Item0);

            // assert item 1 position
            Vector2i item1Position = VerticalContainer.GetNextWidgetPosition();
            Assert.True(item1Position == new Vector2i(0,110));
            VerticalContainer.PushWidget(Item1);

            // assert container
            Assert.True(VerticalContainer.Size == new Vector2i(150, 270));
            Assert.True(VerticalContainer.paddingOffset == new Vector2i(0, 270));
            
            RootWidget.PushWidget(VerticalContainer);


            Assert.True(VerticalContainer.RelativePosition == new Vector2i(0, 0) );
            Assert.True(VerticalContainer.Position == new Vector2i(100, 100) );

        }

        /// Assert container 2
        {
            // assert item 2 position
            Vector2i item2Position = VerticalContainer2.GetNextWidgetPosition();
            Assert.True(item2Position ==  new Vector2i(0, 0));
            VerticalContainer2.PushWidget(Item2);

            // assert item 3 position
            Vector2i item3Position = VerticalContainer2.GetNextWidgetPosition();
            Assert.True(item3Position ==  new Vector2i(0,160));
            VerticalContainer2.PushWidget(Item3);

            // assert container
            Assert.True(VerticalContainer2.Size == new Vector2i(150, 270));
            Assert.True(VerticalContainer2.paddingOffset == new Vector2i(0, 270));

            RootWidget.PushWidget(VerticalContainer2);

            Assert.True(VerticalContainer2.RelativePosition == new Vector2i(170, 0));
            Assert.True(VerticalContainer2.Position == new Vector2i(270, 100));
        }

        // assert root size
        {
            Assert.True(RootWidget.Size == new Vector2i(340, 270));
            Assert.True(RootWidget.paddingOffset == new Vector2i(340, 0));
        }
    }

    [Fact]
    public void TestGrow(){
        Widget Container = new Widget();
        Widget SomeSizing = new Widget();
        Widget growContainer = new Widget();

        Container.styles.Set("Margin", new Vector2i(10,10));

        growContainer.styles.Set("Grow", 1);
        growContainer.styles.Set("PaddingDirection", paddingDirecton.horizontal);

        SomeSizing.Size = new Vector2i(100,100);

        Container.PushWidget(SomeSizing);
        Container.PushWidget(growContainer);

        Container.UpdateStyles();

        Assert.True(growContainer.TotalSize == Container.Size * new Vector2i(1,0));
    }
    
    [Fact]
    public void TestAnchor(){
        Widget Container = new Widget();

        Widget SubContainerR = new Widget();
        Widget SubContainerL = new Widget();
        Widget SubContainerT = new Widget();
        Widget SubContainerB = new Widget();
        Widget SubContainerCX = new Widget();
        Widget SubContainerCY = new Widget();

        SubContainerR.Size = new Vector2i(100,100);
        SubContainerL.Size = new Vector2i(100,100);
        SubContainerT.Size = new Vector2i(100,100);
        SubContainerB.Size = new Vector2i(100,100);
        SubContainerCX.Size = new Vector2i(100,100);
        SubContainerCY.Size = new Vector2i(100,100);

        Widget Right = new Widget();
        Widget Left = new Widget();
        Widget Bottom = new Widget();
        Widget Top = new Widget();
        Widget CenterX = new Widget();
        Widget CenterY = new Widget();

        Right.Size = new Vector2i(10,10);
        Left.Size = new Vector2i(10,10);
        Bottom.Size = new Vector2i(10,10);
        Top.Size = new Vector2i(10,10);
        CenterX.Size = new Vector2i(10,10);
        CenterY.Size = new Vector2i(10,10);

        Right.styles.Set("AnchorX", AnchorX.right);
        Left.styles.Set("AnchorX", AnchorX.left);
        Bottom.styles.Set("AnchorY", AnchorY.bottom);
        Bottom.styles.Set("AnchorX", AnchorX.center);
        Top.styles.Set("AnchorY", AnchorY.top);
        Top.styles.Set("AnchorX", AnchorX.center);
        CenterX.styles.Set("AnchorX", AnchorX.center);
        CenterY.styles.Set("AnchorY", AnchorY.center);

        SubContainerR.PushWidget(Right);
        SubContainerL.PushWidget(Left);
        SubContainerT.PushWidget(Top);
        SubContainerB.PushWidget(Bottom);
        SubContainerCX.PushWidget(CenterX);
        SubContainerCY.PushWidget(CenterY);

    
        Container.PushWidget(SubContainerR);
        Container.PushWidget(SubContainerL);
        Container.PushWidget(SubContainerT);
        Container.PushWidget(SubContainerB);
        Container.PushWidget(SubContainerCX);
        Container.PushWidget(SubContainerCY);

        Container.UpdateStyles();

        Assert.True(Right.RelativePosition == new Vector2i(100 - 10, 0));
        Assert.True(Left.RelativePosition == new Vector2i(0, 0));
        Assert.True(Bottom.RelativePosition == new Vector2i(50 - 5, 100 - 10));
        Assert.True(Top.RelativePosition == new Vector2i( 50 - 5, 0));
        Assert.True(CenterX.RelativePosition == new Vector2i(50-5, 0));
        Assert.True(CenterY.RelativePosition == new Vector2i(0, 50-5));
    }
}