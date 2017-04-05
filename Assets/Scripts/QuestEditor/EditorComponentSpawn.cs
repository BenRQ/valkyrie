using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorComponentSpawn : EditorComponent
{
    QuestData.Spawn monsterComponent;

    DialogBoxEditable uniqueTitleDBE;
    DialogBoxEditable uniqueTextDBE;

    EditorSelectionList monsterTypeESL;
    EditorSelectionList monsterTraitESL;

    public EditorComponentSpawn(string nameIn) : base()
    {
        Game game = Game.Get();
        monsterComponent = game.quest.qd.components[nameIn] as QuestData.Spawn;
        component = monsterComponent;
        name = component.sectionName;
        Update();
    }
    
    override public void Update()
    {
        base.Update();
        CameraController.SetCamera(monsterComponent.location);
        Game game = Game.Get();

        TextButton tb = new TextButton(new Vector2(0, 0), new Vector2(3, 1), "Spawn", delegate { QuestEditorData.TypeSelect(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.button.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleRight;
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(3, 0), new Vector2(16, 1), name.Substring("Spawn".Length), delegate { QuestEditorData.ListSpawn(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.button.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleLeft;
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, 0), new Vector2(1, 1), "E", delegate { Rename(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");


        DialogBox db = new DialogBox(new Vector2(0, 2), new Vector2(4, 1), "Position");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(4, 2), new Vector2(1, 1), "><", delegate { GetPosition(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(5, 2), new Vector2(1, 1), "~", delegate { GetPosition(false); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        if (!monsterComponent.locationSpecified)
        {
            tb = new TextButton(new Vector2(7, 2), new Vector2(4, 1), "Unused", delegate { PositionTypeCycle(); });
        }
        else
        {
            tb = new TextButton(new Vector2(7, 2), new Vector2(4, 1), "Highlight", delegate { PositionTypeCycle(); });
        }
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(0, 4), new Vector2(8, 1), "Event", delegate { QuestEditorData.SelectAsEvent(name); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        if (game.gameType is D2EGameType)
        {
            tb = new TextButton(new Vector2(12, 4), new Vector2(8, 1), "Placement", delegate { QuestEditorData.SelectAsSpawnPlacement(name); });
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
            tb.ApplyTag("editor");
        
            if (monsterComponent.unique)
            {
                tb = new TextButton(new Vector2(0, 6), new Vector2(8, 1), "Unique", delegate { UniqueToggle(); });
            }
            else
            {
                tb = new TextButton(new Vector2(0, 6), new Vector2(8, 1), "Normal", delegate { UniqueToggle(); });
            }
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
            tb.ApplyTag("editor");

            db = new DialogBox(new Vector2(0, 8), new Vector2(5, 1), "Unique Title:");
            db.ApplyTag("editor");

            uniqueTitleDBE = new DialogBoxEditable(new Vector2(5, 8), new Vector2(15, 1), monsterComponent.uniqueTitle, delegate { UpdateUniqueTitle(); });
            uniqueTitleDBE.ApplyTag("editor");
            uniqueTitleDBE.AddBorder();

            db = new DialogBox(new Vector2(0, 10), new Vector2(20, 1), "Unique Information:");
            db.ApplyTag("editor");

            uniqueTextDBE = new DialogBoxEditable(new Vector2(0, 11), new Vector2(20, 8), monsterComponent.uniqueText, delegate { UpdateUniqueText(); });
            uniqueTextDBE.ApplyTag("editor");
            uniqueTextDBE.AddBorder();
        }

        db = new DialogBox(new Vector2(0, 20), new Vector2(3, 1), "Types:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(12, 20), new Vector2(1, 1), "+", delegate { MonsterTypeAdd(0); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        int i = 0;
        for (i = 0; i < 8; i++)
        {
            if (monsterComponent.mTypes.Length > i)
            {
                int mSlot = i;
                string mName = monsterComponent.mTypes[i];
                if (mName.IndexOf("Monster") == 0)
                {
                    mName = mName.Substring("Monster".Length);
                }

                tb = new TextButton(new Vector2(0, 21 + i), new Vector2(1, 1), "-", delegate { MonsterTypeRemove(mSlot); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");

                tb = new TextButton(new Vector2(1, 21 + i), new Vector2(11, 1), mName, delegate { MonsterTypeReplace(mSlot); });
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");

                tb = new TextButton(new Vector2(12, 21 + i), new Vector2(1, 1), "+", delegate { MonsterTypeAdd(mSlot + 1); }, Color.green);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }


        db = new DialogBox(new Vector2(14, 20), new Vector2(3, 1), "Req Traits:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, 20), new Vector2(1, 1), "+", delegate { MonsterTraitsAdd(); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        for (i = 0; i < 8; i++)
        {
            if (monsterComponent.mTraitsRequired.Length > i)
            {
                int mSlot = i;
                string mName = monsterComponent.mTraitsRequired[i];

                tb = new TextButton(new Vector2(14, 21 + i), new Vector2(1, 1), "-", delegate { MonsterTraitsRemove(mSlot); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");

                tb = new TextButton(new Vector2(15, 21 + i), new Vector2(5, 1), mName, delegate { MonsterTraitReplace(mSlot); });
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        db = new DialogBox(new Vector2(14, 21 + monsterComponent.mTraitsRequired.Length), new Vector2(3, 1), "Pool Traits:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, 21 + monsterComponent.mTraitsRequired.Length), new Vector2(1, 1), "+", delegate { MonsterTraitsAdd(true); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        for (int j = 0; j < 8; j++)
        {
            if (monsterComponent.mTraitsPool.Length > j)
            {
                int mSlot = j;
                string mName = monsterComponent.mTraitsPool[j];

                tb = new TextButton(new Vector2(14, 22 + monsterComponent.mTraitsRequired.Length + j), new Vector2(1, 1), "-", delegate { MonsterTraitsRemove(mSlot, true); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");

                tb = new TextButton(new Vector2(15, 22 + monsterComponent.mTraitsRequired.Length + j), new Vector2(5, 1), mName, delegate { MonsterTraitReplace(mSlot, true); });
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        game.tokenBoard.AddHighlight(monsterComponent.location, "MonsterLoc", "editor");
    }

    public void PositionTypeCycle()
    {
        monsterComponent.locationSpecified = !monsterComponent.locationSpecified;
        Update();
    }

    public void UniqueToggle()
    {
        monsterComponent.unique = !monsterComponent.unique;
        Update();
    }


    public void UpdateUniqueTitle()
    {
        monsterComponent.uniqueTitle = uniqueTitleDBE.uiInput.text;
    }

    public void UpdateUniqueText()
    {
        monsterComponent.uniqueText = uniqueTextDBE.uiInput.text;
    }

    public void MonsterTypeAdd(int pos)
    {
        Game game = Game.Get();
        List<EditorSelectionList.SelectionListEntry> monsters = new List<EditorSelectionList.SelectionListEntry>();

        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.CustomMonster)
            {
                monsters.Add(new EditorSelectionList.SelectionListEntry(kv.Key, "Custom"));
            }
        }

        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Spawn)
            {
                monsters.Add(new EditorSelectionList.SelectionListEntry(kv.Key, "Spawn"));
            }
        }

        foreach (KeyValuePair<string, MonsterData> kv in game.cd.monsters)
        {
            string display = kv.Key;
            List<string> sets = new List<string>(kv.Value.traits);
            foreach (string s in kv.Value.sets)
            {
                if (s.Length == 0)
                {
                    sets.Add("base");
                }
                else
                {
                    display += " " + s;
                    sets.Add(s);
                }
            }
            monsters.Add(new EditorSelectionList.SelectionListEntry(display, sets));
        }
        monsterTypeESL = new EditorSelectionList("Select Item", monsters, delegate { SelectMonsterType(pos); });
        monsterTypeESL.SelectItem();
    }

    public void MonsterTypeReplace(int pos)
    {
        Game game = Game.Get();
        List<EditorSelectionList.SelectionListEntry> monsters = new List<EditorSelectionList.SelectionListEntry>();

        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.CustomMonster)
            {
                monsters.Add(new EditorSelectionList.SelectionListEntry(kv.Key, "Quest"));
            }
        }

        foreach (KeyValuePair<string, MonsterData> kv in game.cd.monsters)
        {
            string display = kv.Key;
            List<string> sets = new List<string>(kv.Value.traits);
            foreach (string s in kv.Value.sets)
            {
                if (s.Length == 0)
                {
                    sets.Add("base");
                }
                else
                {
                    display += " " + s;
                    sets.Add(s);
                }
            }
            monsters.Add(new EditorSelectionList.SelectionListEntry(display, sets));
        }
        monsterTypeESL = new EditorSelectionList("Select Item", monsters, delegate { SelectMonsterType(pos, true); });
        monsterTypeESL.SelectItem();
    }

    public void SelectMonsterType(int pos, bool replace = false)
    {
        if (replace)
        {
            monsterComponent.mTypes[pos] = monsterTypeESL.selection.Split(" ".ToCharArray())[0];
        }
        else
        {
            string[] newM = new string[monsterComponent.mTypes.Length + 1];

            int j = 0;
            for (int i = 0; i < newM.Length; i++)
            {
                if (j == pos && i == j)
                {
                    newM[i] = monsterTypeESL.selection.Split(" ".ToCharArray())[0];
                }
                else
                {
                    newM[i] = monsterComponent.mTypes[j];
                    j++;
                }
            }
            monsterComponent.mTypes = newM;
        }
        Update();
    }

    public void MonsterTypeRemove(int pos)
    {
        if ((monsterComponent.mTypes.Length == 1) && (monsterComponent.mTraitsRequired.Length == 0) && (monsterComponent.mTraitsPool.Length == 0))
        {
            return;
        }

        string[] newM = new string[monsterComponent.mTypes.Length - 1];

        int j = 0;
        for (int i = 0; i < monsterComponent.mTypes.Length; i++)
        {
            if (i != pos || i != j)
            {
                newM[j] = monsterComponent.mTypes[i];
                j++;
            }
        }
        monsterComponent.mTypes = newM;
        Update();
    }

    public void MonsterTraitReplace(int pos, bool pool = false)
    {
        Game game = Game.Get();
        HashSet<string> traits = new HashSet<string>();
        foreach (KeyValuePair<string, MonsterData> kv in game.cd.monsters)
        {
            foreach (string s in kv.Value.traits)
            {
                traits.Add(s);
            }
        }
        List<EditorSelectionList.SelectionListEntry> list = new List<EditorSelectionList.SelectionListEntry>();
        foreach (string s in traits)
        {
            list.Add(new EditorSelectionList.SelectionListEntry(s));
        }
        monsterTraitESL = new EditorSelectionList("Select Item", list, delegate { SelectMonsterTraitReplace(pos, pool); });
        monsterTraitESL.SelectItem();
    }

    public void SelectMonsterTraitReplace(int pos, bool pool = false)
    {
        if (pool)
        {
            monsterComponent.mTraitsPool[pos] = monsterTraitESL.selection;
        }
        else
        {
            monsterComponent.mTraitsRequired[pos] = monsterTraitESL.selection;
        }
        Update();
    }

    public void MonsterTraitsAdd(bool pool = false)
    {
        Game game = Game.Get();
        HashSet<string> traits = new HashSet<string>();
        foreach (KeyValuePair<string, MonsterData> kv in game.cd.monsters)
        {
            foreach (string s in kv.Value.traits)
            {
                traits.Add(s);
            }
        }

        List<EditorSelectionList.SelectionListEntry> list = new List<EditorSelectionList.SelectionListEntry>();
        foreach (string s in traits)
        {
            list.Add(new EditorSelectionList.SelectionListEntry(s));
        }
        monsterTraitESL = new EditorSelectionList("Select Item", list, delegate { SelectMonsterTrait(pool); });
        monsterTraitESL.SelectItem();
    }

    public void SelectMonsterTrait(bool pool = false)
    {
        if (pool)
        {
            string[] newM = new string[monsterComponent.mTraitsPool.Length + 1];

            int i;
            for (i = 0; i < monsterComponent.mTraitsPool.Length; i++)
            {
                newM[i] = monsterComponent.mTraitsPool[i];
            }

            newM[i] = monsterTraitESL.selection;
            monsterComponent.mTraitsPool = newM;
        }
        else
        {
            string[] newM = new string[monsterComponent.mTraitsRequired.Length + 1];

            int i;
            for (i = 0; i < monsterComponent.mTraitsRequired.Length; i++)
            {
                newM[i] = monsterComponent.mTraitsRequired[i];
            }

            newM[i] = monsterTraitESL.selection;
            monsterComponent.mTraitsRequired = newM;
        }
        Update();
    }

    public void MonsterTraitsRemove(int pos, bool pool = false)
    {
        if ((monsterComponent.mTypes.Length + monsterComponent.mTraitsPool.Length + monsterComponent.mTraitsRequired.Length) <= 1)
        {
            return;
        }
        if (pool)
        {
            string[] newM = new string[monsterComponent.mTraitsPool.Length - 1];

            int j = 0;
            for (int i = 0; i < monsterComponent.mTraitsPool.Length; i++)
            {
                if (i != pos || i != j)
                {
                    newM[j] = monsterComponent.mTraitsPool[i];
                    j++;
                }
            }
            monsterComponent.mTraitsPool = newM;
        }
        else
        {
            string[] newM = new string[monsterComponent.mTraitsRequired.Length - 1];

            int j = 0;
            for (int i = 0; i < monsterComponent.mTraitsRequired.Length; i++)
            {
                if (i != pos || i != j)
                {
                    newM[j] = monsterComponent.mTraitsRequired[i];
                    j++;
                }
            }
            monsterComponent.mTraitsRequired = newM;
        }
        Update();
    }
}