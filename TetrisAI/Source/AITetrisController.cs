﻿using log4net;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using static TetrisCore.Source.BlockObject;
using System;
using System.Data;
using TetrisAI.Source.util;
using Alba.CsConsoleFormat;
using Cell = Alba.CsConsoleFormat.Cell;
using static System.ConsoleColor;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        private ILog logger;
        private TetrisGame Game;
        private Field field;

        private int erodedObjectCells;

        public AITetrisController()
        {
            logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void initialize(TetrisGame game)
        {
            Game = game;
            erodedObjectCells = 0;
        }

        public void InitController(Field field)
        {
            this.field = field;
            field.OnRoundEnd += OnRoundEnd;
            field.OnRoundStart += OnRoundStart;
        }

        public async void OnRoundStart(object sender)
        {
            Field field = (Field)sender;
            ENumDictionary<Directions, List<System.Drawing.Point>> points = field.GetPlaceablePositions(field.Object);
            List<Task<RoundResult>> tasks = points.Select(x => x.Value.Select(y =>ExecuteFieldAsync(field, x.Key, y)))
            .Select(x => x.ToArray())
            .ToArray()
            .ToDimensionalArray()
            .Flatten()
            .ToList();
            this.logger.Debug("Round Start");

            RoundResult[] result = await Task.WhenAll(tasks.Where(x=>x!=null));
            var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

            var doc = new Document(
                new Grid
                {
                    Color = Gray,
                    Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Auto },
                    Children = {
                        new Cell("Direction") { Stroke = headerThickness },
                        new Cell("Object") { Stroke = headerThickness },
                        new Cell("Point") { Stroke = headerThickness },
                        new Cell("Evaluation") { Stroke = headerThickness },
                        result.Select(item => new[] {
                            new Cell(item.Object.Direction){ Align = Align.Center},
                            new Cell(item.Object.ToString()),
                            new Cell(item.FieldAtEnd.ObjectPoint),
                            new Cell(0) { Align = Align.Center },
                        })
                    }
                }
            );
            string text = ConsoleRenderer.RenderDocumentToText(doc,new TextRenderTarget());
            logger.Debug("\n"+text);
            //foreach(var v in result) logger.Debug(v);
        }
        private Task<RoundResult> ExecuteFieldAsync(Field field, Directions direction, System.Drawing.Point point)
        {
            field = (Field)field.Clone();
            var tcs = new TaskCompletionSource<RoundResult>();
            field.OnRoundEnd += (object sender, RoundResult result) => 
            {
                tcs.TrySetResult(result);
            };
            if (!field.Rotate((int)direction) || !field.CanMoveTo(field.Object,point)) tcs.TrySetCanceled();
            field.PlaceAt(point);

            return tcs.Task;
        }

        public void OnRoundEnd(object sender, RoundResult result)
        {

        }

        public void OnTimerTick()
        {
        }
    }
}