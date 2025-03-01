using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using CSScripting;
using Newtonsoft.Json.Linq;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ButtonList() : Widget{


    public void Random(){
        var childCount = Childrens.Count;
        var random = new Random().Next(0, childCount);
        Childrens[random].Click();
    }
}

public class SelectorWidget<T> : Widget {

    public System.Action<SelectorWidget<T>>? OnChange;
    public Dictionary<string, T> items = new();
    public int Index;
    public TextWidget textWidget = new("", 20);


    public SelectorWidget(Dictionary<string, T> items, System.Action<SelectorWidget<T>>? OnChange = null){
        this.items = items;
        this.Index = 0;

        if (OnChange != null)
            this.OnChange = OnChange;

        if (items.Keys.Count < 1)
            return;

        textWidget = new TextWidget(items.Keys.ToList()[0], 20);

        var arrowLeft = new ButtonSprite(
            new SpriteRenderer(new Sprite("res://hud/arrow-left-button.png")),
            () => { Previous();}
        );

        var arrowRight = new ButtonSprite(
            new SpriteRenderer(new Sprite("res://hud/arrow-right-button.png")),
            () => { Next();}
        );

        arrowLeft.PositionOffset = PositionOffset.CenterLeft;
        arrowRight.PositionOffset = PositionOffset.CenterRight;
        textWidget.PositionOffset = PositionOffset.CenterHorizontal;

        var growContainer = new HorizontalContainer(
            arrowLeft,
            textWidget,
            arrowRight
        );

        growContainer.Grow = new Grow{
            growX = true
        };

        AddChild(growContainer);
    }

    public T GetValue(){
        return items.Values.ToList()[Index];
    }

    public void Next(){
        if (this.Index + 1 <= items.Count -1){
            Index++;
            textWidget.Text = items.Keys.ToList()[Index];
            OnChange?.Invoke(this);
        }
    }

    public void Reset(){
        Index = 0;
        if (items.Count > 0){
            textWidget.Text = items.Keys.ToList()[Index];
        }
        else {
            textWidget.Text = "none";
        }
        OnChange?.Invoke(this);
    }

    public void Random(){
        var itemsCount = items.Count;

        var random = new Random().Next(0, itemsCount);
        Index = random;
        textWidget.Text = items.Keys.ToList()[Index];

        OnChange?.Invoke(this);
    }

    public void Previous(){
        if (Index - 1 >= 0){
            Index--;
            textWidget.Text = items.Keys.ToList()[Index];
            OnChange?.Invoke(this);
        }
    }

}

public class CharacterCreationMenu : Widget {
    
    // state
    Actor Character;
    Object? currentRace;
    Color currentHairColor;
    Color currentSkinColor;

    // widgets
    SelectorWidget<Asset> RaceSelector;
    SelectorWidget<int> GenreSelector;
    SelectorWidget<Asset?> HairSelector;
    SelectorWidget<Asset?> BeardSelector;

    ButtonList HairColorButtons;
    ButtonList SkinColorButtons;

    // data
    Dictionary<string, Asset> Races;
    Dictionary<string, Asset?> Hairs;
    Dictionary<string, Asset?> Beards;

    ObjectRendererWidget CharacterRenderer;

    public CharacterCreationMenu(){
        this.TotalSize = new Vector2i(300, 400);
        this.id = "Character-Creation-Menu";
        this.margin = new Margin{
            bottom = 10, left = 10, right = 10, top = 10
        };
        this.CanDrag  =true;

        currentHairColor = Color.White;
        backgroundColor = new Color(25,25,25,150);

        Character = CreateBaseCharacter();
        Character.Load();
        Character.Scale = new Vector2(4,4);

        CharacterRenderer = new ObjectRendererWidget(Character);
        CharacterRenderer.PositionOffset = PositionOffset.CenterBottom;
        CharacterRenderer.margin = new Margin{
            bottom = 10, left = 10, top = 10, right = 10
        };

        Widget PreviewContainer = new Widget();
        PreviewContainer.PositionOffset = PositionOffset.CenterHorizontal;

        PreviewContainer.TotalSize = new Vector2i(200,200);
        PreviewContainer.AddChild(CharacterRenderer);

        // Prepare Data
        Races = new();
        Hairs = new();
        Beards = new();

        PrepareData();
        Debug.Assert(Races.Count > 0);

        // selectors
        BeardSelector = new SelectorWidget<Asset?>(Beards, (widget) => {
            SetBeard(widget.GetValue());
        });

        BeardSelector.TotalSize = new Vector2i(this.TotalSize.X, BeardSelector.TotalSize.Y);

        GenreSelector = new SelectorWidget<int>(
            new Dictionary<string, int>(){
                {"Masculine", 0},
                {"Feminine", 1}
            },
            (widget) => {
                SetGenre(widget.GetValue());

                if (widget.GetValue() == 1){
                    RemoveAvailableBeards();
                    SetBeard(null);
                    BeardSelector.Reset();
                }
                else {
                    if (RaceSelector != null){
                        SetAvailableBeards(RaceSelector.GetValue().id);
                    }
                }
            }
        );

        GenreSelector.TotalSize = new Vector2i(this.TotalSize.X, GenreSelector.TotalSize.Y);

        HairSelector = new SelectorWidget<Asset?>(Hairs, (widget) => {
            SetHair(widget.GetValue());

        });

        HairSelector.TotalSize = new Vector2i(this.TotalSize.X, HairSelector.TotalSize.Y);

        RaceSelector = new SelectorWidget<Asset>(Races, (widget) => {
            SetRace(widget.GetValue());
            SetGenre(0);
            
            SetAvailableHairs(widget.GetValue().id, 0);
            SetAvailableBeards(widget.GetValue().id);
            PopulateHairColorButtons();
            PopulateSkinColorButtons();
            SetDefaultHairsColor();
            SetDefaultSkinColor();

            GenreSelector.Reset();
            HairSelector.Reset();
            BeardSelector.Reset();
        });

        RaceSelector.TotalSize = new Vector2i(this.TotalSize.X, RaceSelector.TotalSize.Y);

        HairColorButtons = new();
        HairColorButtons.flowDirection = FlowDirections.Horizontal;

        SkinColorButtons = new();
        SkinColorButtons.flowDirection = FlowDirections.Horizontal;
        
        // Default values
        SetRace(Races.Values.ToList()[0]);
        SetGenre(0);
        PopulateHairColorButtons();
        PopulateSkinColorButtons();
        SetDefaultHairsColor();
        SetDefaultSkinColor();

        AddChild(
            new TextWidget("Character Creation Menu", 22)
        );

        AddChild(PreviewContainer);

        AddChild(new TextWidget("Select Your Race", 20));

        AddChild(RaceSelector);

        AddChild(new TextWidget("Select Your Genre", 20));

        AddChild(GenreSelector);

        AddChild(new TextWidget("Change Your appearence", 20));

        AddChild(HairSelector);

        AddChild(BeardSelector);

        AddChild(new TextWidget("Hair Color", 20));

        AddChild(HairColorButtons);

        AddChild(new TextWidget("Skin Color", 20));

        AddChild(SkinColorButtons);

        var okButton = new ButtonText("Ok", () => {
            var raceObject = Character.GetChildId("player-race");


            if (raceObject != null){
                var raceComponent = raceObject.GetComponent<RaceComponent>();

                if (raceComponent != null){
                    Character.Offset = new Vector2((raceComponent.currentSprite.Width / 2) -32 / 2, -(raceComponent.currentSprite.Height - 24));
                    Character.flipOffset = new Vector2((raceComponent.currentSprite.Width / 2) -32 /2, -(raceComponent.currentSprite.Height - 24));
                }

            }

            Character.Scale = new Vector2(1,1);
            Character.AddComponent(new PlayerControler());
            Character.AddComponent(new InventoryHud());
            Character.AddComponent(new EquipmentList());

            Character.Load();
            Character.CellPosition = new Vector2i(0,0);

            State.world.AddChild(Character);
            State.Camera.target = Character;
            State.player = Character;

            State.RootWidget.RemoveById("Character-Creation-Menu");
        });

        var flipButton = new ButtonText("Flip", () => {
            Character.Flip(!Character.IsFlipped);
        });

        var randomButton = new ButtonText("Random", () => {
            RaceSelector.Random();
            GenreSelector.Random();
            HairSelector.Random();
            BeardSelector.Random();
            HairColorButtons.Random();
            SkinColorButtons.Random();
        });

        var buttonsContainer = new HorizontalContainer(
            flipButton,
            randomButton,
            okButton
        );

        buttonsContainer.PositionOffset = PositionOffset.Right;

        AddChild(buttonsContainer);
    }

    public void PopulateHairColorButtons(){
        HairColorButtons.RemoveAllChild();

        var currentRaceAsset = RaceSelector.GetValue();
        var raceInstance = currentRaceAsset.Instanciate();

        var raceComponent = raceInstance.GetComponent<RaceComponent>();

        if (raceComponent != null){
            var hairColors = raceComponent.hairColors;

            foreach (var hairColor in hairColors){
                HairColorButtons.AddChild(
                    new ColorButton(hairColor, () => {
                        SetHairsColor(hairColor);
                    })
                );
            }
        }
    }

    public void PopulateSkinColorButtons(){
        SkinColorButtons.RemoveAllChild();

        var currentRaceAsset = RaceSelector.GetValue();
        var raceInstance = currentRaceAsset.Instanciate();

        var raceComponent = raceInstance.GetComponent<RaceComponent>();

        if (raceComponent != null){
            var skinColors = raceComponent.skinColors;

            foreach (var skinColor in skinColors){
                SkinColorButtons.AddChild(
                    new ColorButton(skinColor, () => {
                        SetSkinColor(skinColor);
                    })
                );
            }
        }
    }

    public void SetDefaultHairsColor(){
        var currentRaceAsset = RaceSelector.GetValue();
        var raceInstance = currentRaceAsset.Instanciate();

        var raceComponent = raceInstance.GetComponent<RaceComponent>();

        if (raceComponent != null){
            var hairColors = raceComponent.hairColors;

            if (hairColors.Length > 0){
                SetHairsColor(hairColors[0]);
            }
        }
    }

    public void SetDefaultSkinColor(){
        var currentRaceAsset = RaceSelector.GetValue();
        var raceInstance = currentRaceAsset.Instanciate();

        var raceComponent = raceInstance.GetComponent<RaceComponent>();

        if (raceComponent != null){
            var skinColors = raceComponent.skinColors;

            if (skinColors.Length > 0){
                SetSkinColor(skinColors[0]);
            }
        }
    }

    void SetAvailableHairs(string raceId, int genre){
        var HairsAssets = State.packageManager.GetAssetByTags("hair");

        HairsAssets.Sort((a, b) => {
            bool aContains = a.id.ToLower().Contains(raceId);
            bool bContains = b.id.ToLower().Contains(raceId);

            if (aContains && !bContains) return -1; // a deve vir antes de b
            if (!aContains && bContains) return 1;  // b deve vir antes de a

            // Desempate, se necessário (exemplo: ordem alfabética pelo id)
            return string.Compare(a.id, b.id, StringComparison.OrdinalIgnoreCase);
        });
        Dictionary<string, Asset?> AvailableHairs = new(); 

        AvailableHairs["none"] = null;

        foreach (var hairAsset in HairsAssets){

            var hairComponent = hairAsset.GetComponent("Hair");

            if (hairComponent == null)
                continue;

            var racesId = ((JArray)hairComponent.parameters["racesId"]).ToObject<string[]>();
            
            if (racesId == null)
                continue;

            if (Array.IndexOf(racesId, raceId) > -1){
                // contains raceid
                AvailableHairs[hairAsset.Name] = hairAsset;
            }
        }

        Hairs = AvailableHairs;

        if (HairSelector != null)
            HairSelector.items = Hairs;
    }

    void RemoveAvailableBeards(){
        Beards = new();

        Beards["none"] = null;

        if (BeardSelector != null)
            BeardSelector.items = Beards;
    }
    void SetAvailableBeards(string raceId){
        var BeardsAssets = State.packageManager.GetAssetByTags("beard");

        BeardsAssets.Sort((a, b) => {
            bool aContains = a.id.ToLower().Contains(raceId);
            bool bContains = b.id.ToLower().Contains(raceId);

            if (aContains && !bContains) return -1; // a deve vir antes de b
            if (!aContains && bContains) return 1;  // b deve vir antes de a

            // Desempate, se necessário (exemplo: ordem alfabética pelo id)
            return string.Compare(a.id, b.id, StringComparison.OrdinalIgnoreCase);
        });

        Dictionary<string, Asset?> AvailableBeards = new(); 

        AvailableBeards["none"] = null;

        foreach (var beardAsset in BeardsAssets){

            var BeardComponent = beardAsset.GetComponent("Beard");

            if (BeardComponent == null)
                continue;

            var racesId = ((JArray)BeardComponent.parameters["racesId"]).ToObject<string[]>();
            
            if (racesId == null)
                continue;

            if (Array.IndexOf(racesId, raceId) > -1){
                // contains raceid
                AvailableBeards[beardAsset.Name] = beardAsset;
            }
        }

        Beards = AvailableBeards;

        if (BeardSelector != null)
            BeardSelector.items = Beards;
    }

    public void PrepareData(){

        var racesAssets = State.packageManager.GetAssetByTags("race");
        racesAssets.Sort((a, b) => {
            if (a.Name.StartsWith("Human")) return -1;
            if (b.Name.StartsWith("Human")) return 1;

            return 0;
        });

        foreach (var race in racesAssets){
            Races[race.Name] = race;
        }

        SetAvailableHairs(Races.Values.ToList()[0].id, 0);
        SetAvailableBeards(Races.Values.ToList()[0].id);
    }

    public void SetRace(Asset asset){
        var raceObject = Character.GetChildId("player-race");

        if (raceObject == null){
            raceObject = new Object("race", "player-race");
            Character.AddChild(raceObject);

            raceObject.Offset = new Vector2(0,0);
            raceObject.RelativePosition = new Vector2(0,0);
        }

        raceObject.RemoveComponent<RaceComponent>();

        currentRace = asset.Instanciate();
        var RaceComponent = currentRace.GetComponent<RaceComponent>();


        if (RaceComponent != null){
            raceObject.AddComponent(RaceComponent);
            raceObject.Load();
        }

        CharacterRenderer.SetObject(Character);
        
        SetGenre(0);
        UpdatePositions();
    }

    public void SetGenre(int genre){
        var raceObject = Character.GetChildId("player-race");

        if (raceObject != null){
            var RaceComponent = raceObject.GetComponent<RaceComponent>();
            
            if (RaceComponent == null)
                return;

            if (genre == 0){
                RaceComponent.currentSprite = RaceComponent.SpriteMasculine;
            }
            else if (genre == 1) {
                RaceComponent.currentSprite = RaceComponent.SpriteFeminine;
            }

            RaceComponent.currentSprite.Color = currentSkinColor;

            RaceComponent.Load();
        }
    }

    public void SetHair(Asset? asset){
        var hairObject = Character.GetChildId("player-hair");
        var raceObject = Character.GetChildId("player-race");

        if (hairObject == null){
            hairObject = new Object("hair", "player-hair");
            hairObject.AddComponent(new SizeComponent());
            Character.AddChild(hairObject);
        }

        hairObject.RemoveComponent<Hair>();

        if (asset != null && raceObject != null){
            var hairObjectAsset = asset.Instanciate();
            var hairCOmponent = hairObjectAsset.GetComponent<Hair>();
            var raceComponent = raceObject.GetComponent<RaceComponent>();

            if (hairCOmponent != null && raceComponent != null){
                hairCOmponent.sprite.Color = currentHairColor;
                
                hairObject.AddComponent(hairCOmponent);
                hairCOmponent.Load();

                var offset = hairCOmponent.GetRaceOffset(RaceSelector.GetValue().id);

                hairObject.Offset = offset;
                hairObject.flipOffset = new Vector2(
                    raceComponent.currentSprite.Width - hairCOmponent.sprite.Width - offset.X,
                    offset.Y
                );
            }
        }
    }

    public void SetHairsColor(Color color){
        var hairObject = Character.GetChildId("player-hair");
        var beardObject = Character.GetChildId("player-beard");

        if (hairObject != null){
            var hairComponent = hairObject.GetComponent<Hair>();

            if (hairComponent != null){
                hairComponent.sprite.Color = color;
            }
        }

        if (beardObject != null){
            var beardComponent = beardObject.GetComponent<Beard>();

            if (beardComponent != null){
                beardComponent.sprite.Color = color;
            }
        }

        currentHairColor = color;
    }

    public void SetSkinColor(Color color){
        var raceObject = Character.GetChildId("player-race");
        if (raceObject != null){
            var raceComponent = raceObject.GetComponent<RaceComponent>();

            currentSkinColor = color;

            if (raceComponent != null){
                raceComponent.currentSprite.Color = color;
            }
        }
    }

    public void SetBeard(Asset? asset){
        var beardObject = Character.GetChildId("player-beard");
        var raceObject = Character.GetChildId("player-race");

        if (beardObject == null){
            beardObject = new Object("beard", "player-beard");
            beardObject.AddComponent(new SizeComponent());
            Character.AddChild(beardObject);
        }

        beardObject.RemoveComponent<Beard>();

        if (asset != null && raceObject != null){
            var beardObjectAsset = asset.Instanciate();
            var beardComponent = beardObjectAsset.GetComponent<Beard>();
            var raceComponent = raceObject.GetComponent<RaceComponent>();


            if (beardComponent != null && raceComponent != null){
                beardObject.AddComponent(beardComponent);
                beardComponent.sprite.Color = currentHairColor;
                beardComponent.Load();

                var offset = beardComponent.GetRaceOffset(RaceSelector.GetValue().id);

                beardObject.Offset = offset;
                beardObject.flipOffset = new Vector2(
                    raceComponent.currentSprite.Width -  beardComponent.sprite.Width - offset.X,
                    offset.Y
                );
            }
        }
    }


    public Actor CreateBaseCharacter(){
        return new Actor(
            "player", 
            "player",
            new EquipmentList(),
            new Inventory(),
            new SizeComponent()
        );
    }
}