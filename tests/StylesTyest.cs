using Raylib_cs;
using DarkAgesRPG.GUI;

namespace tests;

public class StyleTest{

    [Fact]
    public void TestStylesMerge(){

        Styles styles1 = Styles.Default();

        // to replace
        styles1.Set("AnchorX", AnchorX.none);
        styles1.Set("AnchorY", AnchorY.center);

        // to not change
        styles1.Set("FontSize", 32);
        styles1.Set("PaddingDirection", paddingDirecton.vertical);

        Styles styles2 = new Styles();

        styles2.Set("AnchorX", AnchorX.left);
        styles2.Set("AnchorY", AnchorY.center);

        styles1.Override(styles2);

        Assert.True(styles1.Get("AnchorX") == AnchorX.left);
        Assert.True(styles1.Get("AnchorY") == AnchorY.center);
        Assert.True(styles1.Get("FontSize") == 32);
        Assert.True(styles1.Get("PaddingDirection") == paddingDirecton.vertical);
    }
}