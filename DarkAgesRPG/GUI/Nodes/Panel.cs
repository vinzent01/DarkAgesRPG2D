using DarkAgesRPG.GUI;
using Raylib_cs;

public class Panel : Widget {

    public Widget CloseButton;

    Color backgroundColor2 = new Color(68, 68, 68, 200);
    Color backgroundColor = new Color(47,47,47,200);
    Color HeaderTextColor = new Color(255,231,112,255);
    Color TextColor = new Color(234,234,234,255);

    public Panel(){
        CloseButton = new();

        RelativePosition = new Vector2i(400,400);

        styles.Set("Color", backgroundColor);
        styles.Set("Margin", new Vector2i(10,10));
        styles.Set("PaddingIncrease", 10);
        styles.Set("RoundNess", 0.1f);

        Widget Header = CreateHeader();

        PushWidget(Header);
    }

    public Widget CreateHeader(){
        Widget Header = new Widget();

        Header.styles.Set("PaddingDirection", paddingDirecton.horizontal);
        Header.styles.Set("Color", backgroundColor2);
        Header.styles.Set("Grow", 1);

        Text HeaderText = new Text();
        HeaderText.TextString = "Inventory";
        HeaderText.styles.Set("Color", HeaderTextColor);

        CloseButton = new ButtonText("X", 24);
        CloseButton.Size = new Vector2i(30,30);
        CloseButton.styles.Set("AnchorX", AnchorX.right);

        Header.PushWidget(HeaderText);
        Header.PushWidget(CloseButton);

        return Header;
    }

    protected override void OnUpdate()
    {
        ImplementDrag();
    }
}