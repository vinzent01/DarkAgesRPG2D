using System.Numerics;

namespace DarkAgesRPG;

public class SizeComponent : Component{
    public Vector2 Size;

    public Vector2i CalculateTotalSize(Object currentObj)
    {
        // Inicializa com o tamanho do objeto atual
        var totalWidth = 0;
        var totalHeight = 0;

        // Obtém os componentes para calcular o tamanho do objeto atual
        var spriteComponent = currentObj.GetComponent<Sprite>();
        var raceComponent = currentObj.GetComponent<RaceComponent>();

        if (spriteComponent != null)
        {
            totalWidth += (int)(spriteComponent.Width * currentObj.Scale.X);
            totalHeight += (int)(spriteComponent.Height * currentObj.Scale.Y);
        }
        else if (raceComponent != null)
        {
            totalWidth += (int)(raceComponent.currentSprite.Width * currentObj.Scale.X);
            totalHeight += (int)(raceComponent.currentSprite.Height * currentObj.Scale.Y);
        }

        // Soma os tamanhos dos filhos recursivamente
        foreach (var child in currentObj.Children)
        {
            var childSize = CalculateTotalSize(child);
            totalWidth = Math.Max(totalWidth, childSize.X); // Máximo em largura
            totalHeight += childSize.Y; // Soma alturas
        }

        return new Vector2i(totalWidth, totalHeight);
    }

    public override void Load()
    {
        var size = CalculateTotalSize(owner);

        this.Size = new Vector2(size.X, size.Y);

    }
}