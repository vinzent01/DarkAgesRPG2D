
using System.Diagnostics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG.GUI;

public class Widget {

    // Properties
    public Vector2i Position {
        get {
            if (Parent != null){
                return Parent.Position + DragOffset + m_position;
            }
            return DragOffset + m_position;
        }
    }

    public Rectangle Rectangle{
        get {
            return new Rectangle(Position.X, Position.Y, Size.X, Size.Y );
        }
    }

    public Vector2i RelativePosition {
        get {
            return m_position;
        }
        set {
            m_position = value;
        }
    }

    public virtual Vector2i TotalSize{
        get {
            return Size + new Vector2i(MarginOffset.X * 2, MarginOffset.Y * 2);
        }
    }

    public virtual Vector2i Size {
        get {
            return m_size;
        }
        set {
            m_size = value;
        }
    }

    public Vector2i MarginOffset {
        get {
            return styles.Get("Margin");
        }
    }

    // private fields
    private Vector2i m_position;
    private Vector2i DragOffset;
    private Vector2i m_size;
    // public fields
    protected List<Widget> Children;
    public Widget? Parent;
    public Vector2i paddingOffset;
    public Styles styles;

    public Widget(
    ){
        Children = new();
        styles = Styles.Default();
    }

    public Vector2i GetNextWidgetPosition(){
        return   MarginOffset +  paddingOffset;
    }

    private void UpdateChildAnchorPosition(){
        foreach (var c in Children){
            if (c.styles.Get("AnchorX") != AnchorX.none){
                if (c.styles.Get("AnchorX") == AnchorX.left){
                    c.RelativePosition = new Vector2i(0, c.RelativePosition.Y);
                }
                else if (c.styles.Get("AnchorX") == AnchorX.right){
                    c.RelativePosition = new Vector2i(TotalSize.X - c.Size.X, c.RelativePosition.Y);
                }
                else if (c.styles.Get("AnchorX") == AnchorX.center){
                    c.RelativePosition = new Vector2i((TotalSize.X / 2) - (c.Size.X / 2), c.RelativePosition.Y);
                }
            }
            if (c.styles.Get("AnchorY") != AnchorY.none){
                if (c.styles.Get("AnchorY") == AnchorY.top){
                    c.RelativePosition = new Vector2i(c.RelativePosition.X, 0);
                }
                else if (c.styles.Get("AnchorY") == AnchorY.bottom){
                    c.RelativePosition = new Vector2i(c.RelativePosition.X, TotalSize.Y - c.Size.Y);
                }
                else if (c.styles.Get("AnchorY") == AnchorY.center){
                    c.RelativePosition = new Vector2i(c.RelativePosition.X, (TotalSize.Y / 2) - (c.Size.Y / 2));
                }
            }
        }
    }

    private void UpdateChildGrow(){

        foreach (var c in Children){
            if (c.styles.Get("Grow") == 1 ){
                var padding = styles.Get("PaddingIncrease");

                if (c.styles.Get("PaddingDirection") == paddingDirecton.vertical){
                    c.Size = new Vector2i(c.Size.X , Size.Y );
                }
                else if (c.styles.Get("PaddingDirection") == paddingDirecton.horizontal){
                    c.Size = new Vector2i(Size.X , c.Size.Y);
                }
            }
        }
    }

    public void UpdateStyles(){
        //RemovePadding();
        UpdateChildGrow();
        UpdateChildAnchorPosition();

        foreach (var c in Children){
            c.UpdateStyles();
        }
    }

    public static Vector2i GetPaddingDirectionVector2i(paddingDirecton directon){
        if (directon == paddingDirecton.vertical)
            return new Vector2i(0, 1);
        else
            return new Vector2i(1,0);
    }

    private void PushPadding(){
        Vector2i paddingVector2i = GetPaddingDirectionVector2i(styles.Get("PaddingDirection"));
        paddingOffset += paddingVector2i * styles.Get("PaddingIncrease");
        Size += paddingVector2i * styles.Get("PaddingIncrease");
    }

    public void RemovePadding(){
        Vector2i paddingVector2i = GetPaddingDirectionVector2i(styles.Get("PaddingDirection"));
        paddingOffset -= paddingVector2i * styles.Get("PaddingIncrease");
        Size -= paddingVector2i * styles.Get("PaddingIncrease");
    }

    private void PushSize(Vector2i widgetSize){
        if (styles.Get("PaddingDirection") == paddingDirecton.vertical){
            paddingOffset.Y += widgetSize.Y;
            Size += new Vector2i(0, widgetSize.Y);
            Size = new Vector2i(Math.Max(Size.X, widgetSize.X), Size.Y);
        }
        else if (styles.Get("PaddingDirection") == paddingDirecton.horizontal){
            paddingOffset.X += widgetSize.X;
            Size += new Vector2i(widgetSize.X, 0);
            Size = new Vector2i(Size.X, Math.Max(Size.Y, widgetSize.Y));
        }
    }

    private void RemoveSize(Vector2i widgetSize){
        if (styles.Get("PaddingDirection") == paddingDirecton.vertical){
            paddingOffset.Y -= widgetSize.Y;
            Size -= new Vector2i(0, widgetSize.Y);
        }
        else if (styles.Get("PaddingDirection") == paddingDirecton.horizontal){
            paddingOffset.X -= widgetSize.X;
            Size -= new Vector2i(widgetSize.X, 0);
        }
    }

    public bool hasWidget(Widget widget){

        foreach (var c in Children){
            if (c == widget){
                return true;
            }
        }

        return false;
    }

    public void PushWidget(Widget widget){
        Debug.Assert(widget != null);
        widget.Parent = this;

        if (hasWidget(widget))
            return;

        Children.Add(widget);
        widget.RelativePosition = GetNextWidgetPosition();
        

        if (
            widget.styles.Get("AnchorX") == AnchorX.none && 
            widget.styles.Get("AnchorY") == AnchorY.none
        ){
            PushSize(widget.TotalSize);
            PushPadding();
        }
        else {

            if (Size.X < widget.TotalSize.X && Size.Y < widget.TotalSize.Y){
                PushSize(widget.TotalSize);
            }
        }
    }

    public void RemoveWidget(Widget widget){
        Debug.Assert(widget!= null);

        Children.Remove(widget);
        
        if (
            widget.styles.Get("AnchorX") == AnchorX.none && 
            widget.styles.Get("AnchorY") == AnchorY.none
        ){
            RemoveSize(widget.TotalSize);
            RemovePadding();
        }
        else {

            if (Size.X > widget.TotalSize.X && Size.Y > widget.TotalSize.Y){
                RemoveSize(widget.TotalSize);
            }
        }
    }

    public void RemoveWidgets(){
        Children = [];
        paddingOffset = new Vector2i(0,0);
        Size = new Vector2i(0,0);
    }

    public static  Vector2i Center(Rectangle rectangle){
        return new Vector2i(
            (int)(rectangle.X + rectangle.Width / 2f),
            (int)(rectangle.Y + rectangle.Height / 2f)
        );
    }
    
    public static Vector2i CenterText(Rectangle rect, string text, int fontSize){
        Vector2i rectCenter = Center(rect);
        Vector2i textSize = new Vector2i(MeasureText(text, fontSize), fontSize);
        return rectCenter - textSize / 2;
    }


    public bool ContainsPosition(Vector2i pos){
        if (pos.X >= Position.X && pos.X <= Position.X + Size.X){
            if (pos.Y >= Position.Y && pos.Y <= Position.Y + Size.Y){
                return true;
            }
        }
        return false;
    }

    public bool IsHovering(){
        Vector2i MousePosition = new Vector2i(GetMousePosition());

        if (ContainsPosition(MousePosition))
            return true;
        
        return false;
    }

    public bool IsDrag(){
        if (IsHovering()){
            if (IsMouseButtonDown(MouseButton.Left)){
                return true;
            }
        }
        return false;
    }

    public bool IsClicked(){
        if (IsHovering()){
            if (IsMouseButtonPressed(MouseButton.Left)){
                return true;
            }
        }
        return false;
    }

    public bool ImplementDrag(){
        if (IsDrag()){
            Vector2i MouseDelta = new Vector2i(GetMouseDelta());

            DragOffset += MouseDelta;
            return true;
        }
        return false;
    }

    public void Draw(){
        DrawForm();

        foreach (var c in Children){
            c.Draw();
        }
    }
    
    public void Update(){
        OnUpdate();

        foreach (var c in Children){
            c.Update();
        }
    }

    protected virtual void DrawForm(){
        DrawRectangleRounded(new Rectangle(Position.X, Position.Y, TotalSize.X, TotalSize.Y), styles.Get("RoundNess"), 10, styles.Get("Color"));
    }

    protected virtual void OnUpdate(){
        
    }
}

public class Text : Widget{

    public override Vector2i Size{
        get {
            return new Vector2i(
                MeasureText(TextString, styles.Get("FontSize")),
                styles.Get("FontSize")
            );
        }
    }

    public string TextString = "None";

    protected override void DrawForm()
    {
        DrawText(
            TextString,
            Position.X,
            Position.Y,
            styles.Get("FontSize"),
            styles.Get("Color")
        );
    }
}

public class ButtonText : Widget {

    public ButtonText(string text, int fontsize){
        Text textWidget = new();
        textWidget.TextString = text;
        textWidget.styles.Set("AnchorX", AnchorX.center);
        textWidget.styles.Set("AnchorY", AnchorY.center);
        textWidget.styles.Set("Color", Color.White);

        PushWidget(textWidget);
    }
}