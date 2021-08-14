using Verse;
using System;
using System.Text;
using Verse.AI.Group;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using HarmonyLib;

namespace MoreInfo
{
   public static class DebugInfoButtons
    {
        [DebugAction("HSK Debug", "Lord Status", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ShowLordStatus(Pawn p)
		{
            if (p.Dead || p.Downed) { return; }
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Hey! I'm {p.Name.ToStringFull} {p.ageTracker.AgeNumberString} years old. \n");
            var lord = p.GetLord();
            if (lord != null){
                report.AppendLine($"My current Lord is: {lord.ToString()} ({lord.GetUniqueLoadID()}) \n");
                if (!lord.Graph.lordToils.NullOrEmpty() && lord.Graph.lordToils.Count > 0)
                {
                    string label = (lord.CurLordToil == null) ? "(current toil = NULL)" : ("current toil: " + lord.Graph.lordToils.IndexOf(lord.CurLordToil) + " - " + lord.CurLordToil.ToString()) + "\n";
                    report.AppendLine($"About my Lord toils: {label}");
                    int i = 0; 
                    foreach (LordToil toil in lord.Graph.lordToils)
                    {
                        i++;
                        if (toil == null) continue;
                        report.AppendLine($"{i} - toil: {toil.GetType().ToString()}");
                    }
                }
                string label2 = (lord.LordJob.GetReport(p) == null) ? "(lord job report not set up ((NULL)" : lord.LordJob.GetReport(p);
                report.AppendLine($"\nLord Job is: {lord.LordJob.ToString()} ({label2}) \n");
                report.AppendLine("Lord debug info: \n " + lord.DebugString() + "\n");
            } else
            {
                report.AppendLine("I have no active Lord with me :( (Pawn Lord == NULL)");
            }
            Log.Message(report.ToString());
            Find.WindowStack.Add(new Dialog_MessageBox(report.ToString()));
		}

        [DebugAction("HSK Debug", "Job Status", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ShowJobQueue(Pawn p)
		{
            if (p.Dead || p.Downed) { return; }
            StringBuilder report = new StringBuilder();
            report.AppendInNewLine($"Hey! I'm {p.Name.ToStringFull} and i'm busy.\n");
            String line1 = p.CurJob.ToString();
            String line2 = p.jobs.curDriver.ToString();
            report.AppendInNewLine($"My current job is {line1} driven by {line2}.\n");
            report.AppendInNewLine(p.jobs.curDriver.GetReport());
            if (p.jobs.curJob != null)
                report.AppendInNewLine($"Current job has {p.jobs.curJob.count} count\n");
            var queue = p.jobs.jobQueue;
            if (queue != null)
            {
                report.AppendInNewLine($"\nAlso i have {queue.Count} jobs to do in queue.");
                if (queue.Count > 0)
                {
                    report.AppendInNewLine($"Here my queued jobs:");
                    for (int i = 0; i < queue.Count; i++ )
                    {
                        var job = queue[i];
                        if (job != null)
                        {
                            report.AppendInNewLine($"\n{i} - {job.job.ToString()} driven by {job.job.GetCachedDriverDirect.ToString()}");
                            report.AppendInNewLine(job.job.GetCachedDriverDirect.GetReport());
                        }
                    }
                }
            }
			Log.Message(report.ToString());
            Find.WindowStack.Add(new Dialog_MessageBox(report.ToString()));
		}

        //Show Storyteller detailed info 
        [DebugAction("HSK Debug", "Storyteller Info", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void StorytellerFix_GetDebugInfo()
        {
            var reporter = new StringBuilder();
            reporter.AppendInNewLine($"Storyteller: {Find.Storyteller.def.label} \n");
            reporter.AppendInNewLine($"Difficulty: {Find.Storyteller.difficultyDef.label} \n");
            var comps = Find.Storyteller.storytellerComps;
            reporter.AppendInNewLine($"Teller has {comps.Count} comps: \n");
            foreach (var comp in comps)
            {
                reporter.AppendInNewLine($"{comp.ToString()}");
                reporter.AppendInNewLine($"{comp.ToStringDeep(" - ")}");
                reporter.AppendInNewLine($"{comp.props.ToStringDeep("\t")}");
            }
            reporter.AppendInNewLine($"\n Teller basic debug info: \n {Find.Storyteller.DebugString()}");
            reporter.AppendInNewLine($"Time is: {Time.frameCount} \n");
            Find.WindowStack.Add(new Dialog_MessageBox(reporter.ToString()));
        }

        //Show Storyteller events Queue 
        [DebugAction("HSK Debug", "Show Event Queue", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void StorytellerFix_GetInfoButton()
        {
                string teller = Find.Storyteller.def.label;
                string difficulty = Find.Storyteller.difficultyDef.label;
                int queueLength = Find.Storyteller.incidentQueue.Count;
                string queue = Find.Storyteller.incidentQueue.DebugQueueReadout;
                Find.WindowStack.Add(new Dialog_MessageBox($"Storyteller: {teller}. \n\nDifficulty: {difficulty}. \n\nIncidents in the queue: {queueLength}. \n\nIncidents: \n\n {queue}"));
        }
 
        //Clear Storyteller Queue button
        [DebugAction("HSK Debug", "Clear Event Queue", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void StorytellerFix_ClearButton()
        {
                int currentQueueLength = Find.Storyteller.incidentQueue.Count;
                if (currentQueueLength != 0)
                {
                    Find.Storyteller.incidentQueue.Clear();
                }
                int newQueueLength = Find.Storyteller.incidentQueue.Count;
                bool cleared = newQueueLength != currentQueueLength;
                Find.WindowStack.Add(new Dialog_MessageBox($"Current queue length: {currentQueueLength}" + 
                                                         (cleared ? $"\n\nThe queue is cleared. \n\nRemaining incidents in the storyteller queue: {newQueueLength} \n" : $"\n\nThe queue is NOT cleared. \n\nThe number of incidents has not changed. \n\nRemaining incidents in the storyteller queue: {newQueueLength} \n" )));
        }

        //Show current map lords statistics
        [DebugAction("HSK Debug", "Show lords statistics", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void LordsStatistic()
        {
                var mapName = Find.CurrentMap.ToString();
                var lordsCount = Find.CurrentMap.lordManager.lords.Count;
                var emptyLords = from lord in Find.CurrentMap.lordManager.lords
                                    where lord.ownedPawns.NullOrEmpty()
                                    select lord;
                var emptyBuilds = from lord in emptyLords
                                    where lord.ownedBuildings.NullOrEmpty()
                                    select lord;
                Find.WindowStack.Add(new Dialog_MessageBox($"Map: {mapName}. \n\nLord Manager has: {lordsCount} lords. \n\nThere are: {emptyLords.Count()} lords with null or empty pawn list. \n\nIt also:  {emptyBuilds.Count()} lords with null or empty buildings list."));
        }

        //Clear current map empty lords
        [DebugAction("HSK Debug", "Clear empty lords", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ClearEmptyLords()
        {
                var mapName = Find.CurrentMap.ToString();
                int lordsCount = 0;
                int emptyLordCount = 0;
                int cleanedLords = 0;
                if (!Find.CurrentMap.lordManager.lords.NullOrEmpty() && Find.CurrentMap.lordManager.lords.Count > 0)
                {
                    lordsCount = Find.CurrentMap.lordManager.lords.Count;
                    var emptyLords = (from lord in Find.CurrentMap.lordManager.lords
                                    where lord.ownedPawns.NullOrEmpty()
                                    select lord).ToList();
                    if (!emptyLords.NullOrEmpty() && emptyLords.Count() > 0)
                    {
                        emptyLordCount = emptyLords.Count();
                        foreach (var lord in emptyLords)
                        {
                            Find.CurrentMap.lordManager.RemoveLord(lord);
                        }
                    }
                    cleanedLords = lordsCount - Find.CurrentMap.lordManager.lords.Count;
                }
                Find.WindowStack.Add(new Dialog_MessageBox($"Map: {mapName}. \n\nLord Manager has: {lordsCount} lords. \n\nThere are: {emptyLordCount} lords with null or empty pawn list. \n\nCleaned {cleanedLords} lords."));
        }

        //Current sky info
        [DebugAction("HSK Debug", "Show sky info", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ShowSkyInfo()
        {
            var mapName = Find.CurrentMap.ToString();
            string skyInfo = Find.CurrentMap.skyManager.DebugString();
            Find.WindowStack.Add(new Dialog_MessageBox($"Map: {mapName}. \n\n {skyInfo}"));
        }

        //Thing comps
        [DebugAction("HSK Debug", "Show thing comps", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ShowThingComps()
        {
            var items = Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell());
            if (!items.EnumerableNullOrEmpty())
            {
                var thing = items.First();
                if (thing != null && thing is ThingWithComps comped)
                {
                    var comps = comped.AllComps;
                    var builder = new StringBuilder();
                    if (!comps.NullOrEmpty())
                        foreach (var comp in comps)
                            {
                                builder.AppendInNewLine(comp.GetType().ToString());
                            }
                    Find.WindowStack.Add(new Dialog_MessageBox(string.Format("{0} ({3}) has {1} comps: \n\n{2}", 
                                thing.LabelCap, 
                                comps.NullOrEmpty() 
                                    ? "null" 
                                    : comps.Count.ToString(),
                                builder.ToString(),
                                thing.GetType().ToString())));
                }
            }
        }

        [DebugAction("HSK Debug", "Show NPS cell data", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ShowNPSCellData()
        {
            var report = new StringBuilder();
            var npsModID ="skyarkhangel.NPS";
            if (ModsConfig.IsActive(npsModID))
            {
                var cellPoint = UI.MouseCell();
                if (cellPoint.InBounds(Find.CurrentMap))
                {
                    var watcherTypeName = "NPS.Watcher";
                    var watcherType = AccessTools.TypeByName(watcherTypeName);
                    if (watcherType != null)
                    {
                        var component = Find.CurrentMap.GetComponent(watcherType);
                        if (component != null)
                        {
                            var cellDataTypeName = "NPS.cellData";
                            var cellDataType = AccessTools.TypeByName(cellDataTypeName);
                            if (cellDataType != null)
                            {
                                var cellsDataFiled = watcherType.GetField("cellWeatherAffects", BindingFlags.Public | BindingFlags.Instance);
                                var cells = cellsDataFiled.GetValue(component);
                                if (cells != null && cells is IDictionary dict)
                                {
                                    var cellData = dict[cellPoint];
                                    if (cellData != null)
                                    {
                                        report.AppendInNewLine($"NPS Watcher cell data:\n");
                                        var terrainDef = cellData.GetType().GetProperty("currentTerrain");
                                        report.AppendInNewLine($"{terrainDef.Name} = {terrainDef.GetValue(cellData).ToString()}");
                                        var fileds = cellData.GetType().GetFields();
                                        foreach (var fi in fileds)
                                        {
                                            report.AppendInNewLine($"{fi.Name} = {fi.GetValue(cellData).ToString()}");
                                        }
                                    }
                                    else
                                        report.AppendInNewLine($"cellData is empty or null");
                                }
                                else
                                    report.AppendInNewLine($"Cannot access to cells dictionary");
                            }
                            else
                                report.AppendInNewLine($"Cannot find type {cellDataTypeName}");
                        }
                        else
                            report.AppendInNewLine($"Current map does not contain Watcher map component");
                    }
                    else
                        report.AppendInNewLine($"{watcherTypeName} type is not found");
                }
                else
                    report.AppendInNewLine($"{cellPoint} is out of bounds");
            }
            else
                report.AppendInNewLine($"{npsModID} is not active");

            Find.WindowStack.Add(new Dialog_MessageBox(report.ToString()));
        }
    }
}
