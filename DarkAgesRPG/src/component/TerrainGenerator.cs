using System.Numerics;
using Raylib_cs;

namespace DarkAgesRPG;

public class TerrainGenerator : Component{
    Vector2i Size;
    Random rng;
    List<Object> Tiles;

    public TerrainGenerator(int width, int height){
        Size = new Vector2i(width, height);
        rng = new Random();
        Tiles = new();
    }

    public override void Draw(){

        foreach (var tile in Tiles){

            if (State.Camera.IsInsideCameraView(tile)){
                tile.Draw();
            }
        }
    }
    
    public override void Load(){
        if (owner == null)
            return;

        List<Vector2i> TreePositions = new();

        var grassTile = State.packageManager.GetAsset("grass-tile");
        var grassObjAsset = State.packageManager.GetAsset("grass-object");
        var pineTree = State.packageManager.GetAsset("pine-tree");

        for (var x = 0; x < Size.X; x++){
            for (var y =0; y < Size.Y; y++){

                if (grassTile != null){
                    var grassTileObj = grassTile.Instanciate();
                    grassTileObj.Load();
                    grassTileObj.IsVisible = true;
                    grassTileObj.CellPosition = new Vector2i(x,y);
                    grassTileObj.DoDrawDebug = false;

                    Tiles.Add(grassTileObj);
                }

                if (grassObjAsset != null){
                    var grassObj = grassObjAsset.Instanciate();
                    grassObj.Load();
                    grassObj.IsVisible = true;
                    grassObj.CellPosition = new Vector2i(x,y);
                    grassObj.DoDrawDebug = false;

                    Tiles.Add(grassObj);
                }

            }
        }

        for (var x = 0; x < Size.X; x++){
            for (var y =0; y < Size.Y; y++){
                if (pineTree != null){
                    var canPlaceTree = true;

                    foreach (var treePosition in TreePositions){
                        if (Vector2i.Distance(treePosition, new Vector2i(x,y)) <= 3){
                            canPlaceTree = false;
                            break;
                        }
                    }

                    if (canPlaceTree){
                        if (rng.Next(0, 4) == 0){
                            var treeObj = pineTree.Instanciate();
                            treeObj.Load();
                            treeObj.IsVisible = true;
                            treeObj.CellPosition = new Vector2i(x,y);

                            owner.AddChild(treeObj);
                            TreePositions.Add(treeObj.CellPosition);
                        }
                    }

                }
            }
        }
    }
}