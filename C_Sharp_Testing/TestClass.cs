﻿#region Referenceing

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Pyrrha;
using Pyrrha.SelectionFilter;
using Pyrrha.Util;
using Pyrrha.Util.Query;

#endregion

namespace C_Sharp_Testing
{
    public class TestClass
    {
        private Transaction MasterTrans;



        


        [CommandMethod( "tst1" )]
        public void test1()
        {
            var ActiveDocument = new Document();
            var MsManager = ActiveDocument.ModelSpaceManager;

            //ActiveDocument.ExecuteQuery();

            //var entities = ActiveDocument.ModelSpaceEntities.Where(ent => ent is Line).ToArray();

            //foreach (Line lineEnt in entities)
            //{
            //    lineEnt.Color = StaticExtenstions.StaticExtenstions.GenerateAutoCadColor(2);
            //}

            //ActiveDocument.ModelSpaceManager.CommitChanges(entities.ToList());
            //BlockReference
            //var sw = new Stopwatch();
            //sw.Start();
            //var allText = ActiveDocument.AllText;
            //var mtext = ActiveDocument.MText;
            //var dbtext = ActiveDocument.DBText;

            //var lineFilter = new LineSelectionFilter( startX: new PointQuery( ">", 4 ) );
            //var textFilter = new TextSelectionFilter {ColorIndex = 3};
            //var lines = ActiveDocument.ModelSpaceEntities.ApplyFilter( lineFilter );
            //var dbText = ActiveDocument.ModelSpaceEntities.ApplyFilter( textFilter );
            //var blocks = ActiveDocument.Blocks;
            var clouds = ActiveDocument.GetEntities( new EntitySelectionFilter( "lwpolyline" ) );
            MsManager.EraseEntities(clouds);

            //foreach ( var blk in blocks.Where( block => block.HasAttribute( "app#" ) ) )
            //{
            //    blk.GetAttribute( "app#" ).TextString = "Success!!";
            //}
            //ActiveDocument.ModelSpaceManager.CommitChanges( blocks );

            //doc.Editor.WriteMessage("{0} text entities processed: {1}\n"
            //                            , ActiveDocument.AllText.Count
            //                            , sw.ElapsedMilliseconds);

            //sw = new Stopwatch();
            //sw.Start();

            //dbtext.ToList().ForEach(ent => ent.Color = StaticExtenstions.GenerateAutoCadColor(3));
            //ActiveDocument.ModelSpaceManager.CommitChanges(dbtext);

            //sw.Stop();
            //doc.Editor.WriteMessage("{0} DbText entities modified: {1}\n" 
            //                        , dbtext.Count
            //                        , sw.ElapsedMilliseconds);
        }
    }
}