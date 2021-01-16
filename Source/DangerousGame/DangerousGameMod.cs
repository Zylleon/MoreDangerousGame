using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;


namespace DangerousGame
{
    public class DangerousGameMod : Mod
    {
        public DangerousGameSettings settings;
        public static DangerousGameMod mod;
        public override string SettingsCategory()
        {
            return "ZDG_ModName".Translate();
        }

        public DangerousGameMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<DangerousGameSettings>();
            mod = this;
        }

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Listing_Standard mainListing = new Listing_Standard();
            mainListing.Begin(inRect);
            DangerousGameSettings settings = DangerousGameMod.mod.settings;

            //Text.Font = GameFont.Medium;
            //GUI.color = new Color(255, 180, 0);
            //mainListing.Label("ZDG_Restart".Translate());

            //GUI.color = Color.white;
            //Text.Font = GameFont.Small;
            //mainListing.Gap();

            // Valid Prey
            mainListing.Label("ZDG_ValidPrey".Translate());
            Rect preyOptionsRect = mainListing.GetRect(30f);
            Rect preySelectRect = preyOptionsRect.LeftPart(0.19f).Rounded();
            Rect preySelectDescRect = preyOptionsRect.RightPart(0.79f).Rounded();

            Listing_Standard preyListing = new Listing_Standard();
            preyListing.Begin(preySelectRect);

            if (preyListing.ButtonText(("ZDG_ValidPrey" + (int)settings.validPrey).Translate()))
            {
                List<FloatMenuOption> preyOptionsList = new List<FloatMenuOption>();
                foreach (ValidPrey prey in Enum.GetValues(typeof(ValidPrey)))
                {
                    preyOptionsList.Add(new FloatMenuOption(("ZDG_ValidPrey" + (int)prey).Translate(), delegate { settings.validPrey = prey; }, MenuOptionPriority.Default));
                }
                Find.WindowStack.Add(new FloatMenu(preyOptionsList));
            }
            preyListing.End();
            Widgets.Label(preySelectDescRect, ("ZDG_ValidPreyDesc" + (int)settings.validPrey).Translate());
            mainListing.Gap();


            // Predator types
            mainListing.Label("ZDG_PredType".Translate());

            Rect predOptionsRect = mainListing.GetRect(30f);
            Rect predSelectRect = predOptionsRect.LeftPart(0.19f).Rounded();
            Rect predSelectDescRect = predOptionsRect.RightPart(0.79f).Rounded();

            Listing_Standard predTypeListing = new Listing_Standard();
            predTypeListing.Begin(predSelectRect);
            if (predTypeListing.ButtonText(("ZDG_PredType" + (int)settings.predType).Translate()))
            {
                List<FloatMenuOption> preyOptionsList = new List<FloatMenuOption>();
                foreach (PredType predType in Enum.GetValues(typeof(PredType)))
                {
                    preyOptionsList.Add(new FloatMenuOption(("ZDG_PredType" + (int)predType).Translate(), delegate { settings.predType = predType; }, MenuOptionPriority.Default));
                }
                Find.WindowStack.Add(new FloatMenu(preyOptionsList));
            }
            predTypeListing.End();
            Widgets.Label(predSelectDescRect, ("ZDG_PredTypeDesc" + (int)settings.predType).Translate());

            mainListing.Gap();

            // revenge chance
            Texture2D harmIcon = ContentFinder<Texture2D>.Get("UI/Designators/Hunt");
            Texture2D tameIcon = ContentFinder<Texture2D>.Get("UI/Designators/Tame");

            Rect harmLabelRect = mainListing.GetRect(Text.LineHeight);

            WidgetRow harmWR = new WidgetRow();
            harmWR.Init(harmLabelRect.xMin, harmLabelRect.yMin);
            harmWR.Icon(harmIcon);
            harmWR.Label("HarmedRevengeChance".Translate());
            mainListing.IntRange(ref settings.revOnDamage, 0, 100);
            mainListing.Gap();

            Rect tameLabelRect = mainListing.GetRect(Text.LineHeight);
            WidgetRow tameWR = new WidgetRow();
            tameWR.Init(tameLabelRect.xMin, tameLabelRect.yMin);
            tameWR.Icon(tameIcon);
            tameWR.Label("TameFailedRevengeChance".Translate());
            mainListing.IntRange(ref settings.revOnTameFail, 0, 100);
            mainListing.Gap();

            Text.Font = GameFont.Medium;
            GUI.color = new Color(255, 180, 0);
            mainListing.Label("ZDG_Restart".Translate());

            GUI.color = Color.white;
            Text.Font = GameFont.Small;

            mainListing.End();

        }

    }



    public class DangerousGameSettings : ModSettings
    {
        public static bool spawnHungry = true;
        public ValidPrey validPrey = ValidPrey.Predators;
        public PredType predType = PredType.MeatEaters;
        public IntRange revOnDamage = new IntRange(50, 100);
        public IntRange revOnTameFail = new IntRange(50, 100);

        public override void ExposeData()
        {
            Scribe_Values.Look(ref spawnHungry, "spawnHungry", true);
            Scribe_Values.Look(ref validPrey, "validPrey", ValidPrey.Predators);
            Scribe_Values.Look(ref predType, "predType", PredType.MeatEaters);
            Scribe_Values.Look(ref revOnDamage, "revOnDamage", new IntRange(20, 100));
            Scribe_Values.Look(ref revOnTameFail, "revOnTameFail", new IntRange(20, 100));

        }
    }

    public enum ValidPrey : byte
    {
        Vanilla,
        Predators,
        Humans,
        Colonists
    }

    public enum PredType : byte
    {
        Vanilla,
        MeatEaters,
        Dangerous,
        Everything
    }
}
