using Alba.CsConsoleFormat;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using static System.ConsoleColor;
using static TetrisAI.Source.Evaluator;
using static TetrisCore.Source.BlockUnit;
using Cell = Alba.CsConsoleFormat.Cell;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        private ILog _logger;
        private TetrisGame _game;
        private Field _field;
        private Evaluator _evaluator;

        //操作間隔:msec
        private int interval;

        public AITetrisController(Evaluator evaluator,int interval=1)
        {
            _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this._evaluator = evaluator;
            this.interval = interval;
        }

        public void initialize(TetrisGame game)
        {
            _game = game;
        }

        public void InitController(Field field)
        {
            _game.TimerEnabled = true;
            this._field = field;
            field.OnRoundEnd += OnRoundEnd;
            field.OnRoundStart += OnRoundStart;
        }

        public async void OnRoundStart(object sender)
        {
            await Task.Run(() =>
            {
                Field field = (Field)sender;
                List<BlockPosition> positions = field.GetPlaceablePositions(field.Object.Unit);
                List<Task<RoundResult>> field_tasks = positions
                    .Select(x => ExecuteFieldAsync(field, x))
                    .ToList();
                this._logger.Debug("Round Start");

                RoundResult[] round_result = Task.WhenAll(field_tasks.Where(x => x.Status != TaskStatus.Canceled)).Result;
                field_tasks.ForEach(x=>x.Dispose());
                /*
                var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);
                var doc = new Document(
                    new Grid
                    {
                        Color = Gray,
                        Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto },
                        Children = {
                        new Cell("Direction") { Stroke = headerThickness },
                        new Cell("Object") { Stroke = headerThickness },
                        new Cell("Point") { Stroke = headerThickness },
                        round_result.Select(item => new[] {
                            new Cell(item.Position.Direction){ Align = Align.Center},
                            new Cell(item.FieldAtEnd.ToString()),
                            new Cell(item.Position.Point),
                        })
                        }
                    }
                );
                try
                {
                    string text = ConsoleRenderer.RenderDocumentToText(doc, new TextRenderTarget());
                    _logger.Debug("\n" + text);
                }
                catch (Exception e) { };*/
                List<Tuple<BlockPosition, EvaluationResult>> results = round_result
                    .Select(x => new Tuple<BlockPosition, EvaluationResult>(x.Position, _evaluator.Evaluate(EvaluationItem.GetEvaluationItem(x))))
                    .OrderByDescending(x => x.Item2.EvaluationValue)
                    .ToList();
                /*
                doc = new Document(
                    new Grid
                    {
                        Color = Gray,
                        Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto },
                        Children = {
                        new Cell("Point") { Stroke = headerThickness },
                        new Cell("Direction") { Stroke = headerThickness },
                        new Cell("Result") { Stroke = headerThickness },
                        results.Select(item => new[] {
                            new Cell(item.Item1.Point),
                            new Cell(item.Item1.Direction),
                            new Cell(item.Item2.EvaluationValue),
                        })
                        }
                    }
                );
                try
                {
                    string text = ConsoleRenderer.RenderDocumentToText(doc, new TextRenderTarget());
                    _logger.Debug("\n" + text);
                }
                catch (Exception e) { };*/
                if (results.Count == 0) return;
                var dest = results
                    .GroupBy(x => x.Item2.EvaluationValue)
                    .First()
                    .GetRandom();
                var task = TryPlaceAsync(field, dest.Item1);
                task.Wait();
                task.Dispose();
            });
        }

        private Task<RoundResult> ExecuteFieldAsync(Field field, BlockPosition position)
        {
            field = (Field)field.Clone();
            var tcs = new TaskCompletionSource<RoundResult>();
            field.OnRoundEnd += (object sender, RoundResult result) =>
            {
                tcs.TrySetResult(result);
            };
            field.OnGameOver += (object sender) =>
            {
                tcs.TrySetCanceled();
            };
            if (!field.Rotate((int)position.Direction) || !field.CanMoveTo(field.Object, position.Point)) tcs.TrySetCanceled();
            field.PlaceAt(position);

            return tcs.Task;
        }
        private Task<bool> TryPlaceAsync(Field field,BlockPosition position)
        {
            var tcs = new TaskCompletionSource<bool>();
            bool placed = false;
            field.OnRoundEnd += (object sender, RoundResult result) =>
            {
                placed = true;
                tcs.TrySetResult(result.Position.Equals(position));
            };
            while (!placed)
            {
                Task.Delay(interval).Wait();
                if (field.Object.Point.X!=position.Point.X)
                {
                    _game.Move((position.Point.X- field.Object.Point.X)>0?Directions.EAST:Directions.WEST);
                }
                Task.Delay(interval).Wait() ;
                if (field.Object.Direction != position.Direction)
                {
                    _game.Rotate(true);
                }
                if (field.Object.Point.X == position.Point.X && field.Object.Direction == position.Direction) break;
            }
            Task.Delay(interval*2).Wait();
            field.PlaceImmediately();
            return tcs.Task;
        }

        public void OnRoundEnd(object sender, RoundResult result)
        {
            //_logger.Info("\n"+result.FieldAtEnd.ToString());
        }
        public void OnTimerTick()
        {
            _game.Move(Directions.SOUTH);
        }
    }
}