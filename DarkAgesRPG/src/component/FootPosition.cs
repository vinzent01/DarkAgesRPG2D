
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

namespace DarkAgesRPG;

public class FootPosition : Component{
    public List<Vector2i> cellsOnGround;

    public FootPosition(){
        cellsOnGround = new();
    }

    public override void Load()
    {
        List<Vector2> FootPositions = GetFootPositions();
        cellsOnGround = CalculateCellPositions(FootPositions);
        SetSpriteOffset();
        UpdateCellPositions();
    }

    public List<Vector2> GetFootPositions(){
        var currentSprite = owner.GetComponent<Sprite>();

        List<Vector2> footPositions = new();

        var bottomLine = (currentSprite.Height/ 32) -1;

        if (currentSprite != null){
            for (var x =0;  x < currentSprite.Width; x+= State.Config.TileSize){
                for (var y =0;  y < currentSprite.Height; y += State.Config.TileSize){
                    Vector2 position = new (x,y);
                    Vector2 endCell = new (State.Config.TileSize-1, State.Config.TileSize-1);
                    var Line = y/32;
                    // if is on bottom
                    if (Line == bottomLine){
                        
                        // if is a complete cell
                        if (position.X + endCell.X <= currentSprite.Width){
                            footPositions.Add(position);
                        }

                    }
                }   
            }
        }

        return footPositions;
    }

    public List<Vector2i> CalculateCellPositions(List<Vector2> positions){
        var currentSprite = owner.GetComponent<Sprite>();

        List<Vector2i> CellPositions = new();

        for (var x = 0; x < currentSprite.Width; x+= State.Config.TileSize){
            for (var y = 0; y < currentSprite.Height; y += State.Config.TileSize){
                foreach (var point in positions){
                    var currentCell = new Vector2(x,y);
                    var nextCell = new Vector2(x+32,y+32);
                    var cellPosition = new Vector2i(x/32, y/32);

                    // point is in between the cells
                    if (
                        point.X < nextCell.X && point.Y < nextCell.Y &&
                        point.X >= currentCell.X && point.Y >= currentCell.Y
                    ){
                        CellPositions.Add(cellPosition);
                    }
                }
            }
        }

        return CellPositions;
    }

    public void SetSpriteOffset(){
        var topLeft = cellsOnGround[0];
        var pixelPos = new Vector2(topLeft.X * State.Config.TileSize, topLeft.Y * State.Config.TileSize);

        var currentSprite = owner.GetComponent<Sprite>();

        if (currentSprite != null){
            owner.Offset = -pixelPos;
            owner.flipOffset = -pixelPos;
        }
    }

    public void UpdateCellPositions(){
        SetSpriteOffset();

        owner.CellPositions.Clear();

        var currentSprite = owner.GetComponent<Sprite>();
        var spriteoffset = new Vector2i(0, (currentSprite.Height / State.Config.TileSize) -1 );


        foreach (var cellOnGround in cellsOnGround){
            var totalPosition = owner.CellPosition + cellOnGround - spriteoffset;
            owner.CellPositions.Add(totalPosition);
        }
    }

}