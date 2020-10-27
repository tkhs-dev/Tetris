using Alba.CsConsoleFormat;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using static System.ConsoleColor;
using static TetrisAI.Source.Evaluation;
using static TetrisCore.Source.BlockUnit;
using Cell = Alba.CsConsoleFormat.Cell;

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
            List<BlockPosition> positions = field.GetPlaceablePositions(field.Object.Unit);
            List<Task<RoundResult>> field_tasks = positions.Select(x => ExecuteFieldAsync(field, x))
            .ToList();
            this.logger.Debug("Round Start");

            RoundResult[] round_result = await Task.WhenAll(field_tasks.Where(x => x != null));
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
                        round_result.Select(item => new[] {
                            new Cell(item.Object.Direction){ Align = Align.Center},
                            new Cell(item.Object.ToString()),
                            new Cell(item.FieldAtEnd.Object.Point),
                            new Cell(0) { Align = Align.Center },
                        })
                    }
                }
            );
            string text = ConsoleRenderer.RenderDocumentToText(doc, new TextRenderTarget());
            logger.Debug("\n" + text);
            EvaluationItem[] evaluation_items = round_result.Select(x=>EvaluationItem.GetEvaluationItem(x)).ToArray();
            List<Task<EvaluationResult>> evaluation_tasks = evaluation_items
                .Select(x => Evaluation.EvaluateAsync(x)).ToList();
            Task.WaitAll(evaluation_tasks.ToArray());
            EvaluationResult[] results = await Task.WhenAll(evaluation_tasks.ToArray());
            doc = new Document(
                new Grid
                {
                    Color = Gray,
                    Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Auto },
                    Children = {
                        new Cell("UUID") { Stroke = headerThickness },
                        new Cell("Point") { Stroke = headerThickness },
                        new Cell("Direction") { Stroke = headerThickness },
                        new Cell("Result") { Stroke = headerThickness },
                        results.OrderByDescending(x=>x.EvaluationValue).Select(item => new[] {
                            new Cell(item.ID){ Align = Align.Center},
                            new Cell(round_result.Where(x=>x.ID.Equals(item.ID)).Select(x=>x.Object.Point).FirstOrDefault()),
                            new Cell(round_result.Where(x=>x.ID.Equals(item.ID)).Select(x=>x.Object.Direction).FirstOrDefault()),
                            new Cell(item.EvaluationValue),
                        })
                    }
                }
            );
            text = ConsoleRenderer.RenderDocumentToText(doc, new TextRenderTarget());
            logger.Debug("\n" + text);
            //EvaluationResult[] evaluation_result = await Task.WaitAll(evaluation_tasks);
            //evaluation_result = evaluation_result.OrderBy(x => x.EvaluationValue).ToArray();
            //foreach(var v in result) logger.Debug(v);
        }

        private Task<RoundResult> ExecuteFieldAsync(Field field, BlockPosition position)
        {
            field = (Field)field.Clone();
            var tcs = new TaskCompletionSource<RoundResult>();
            field.OnRoundEnd += (object sender, RoundResult result) =>
            {
                tcs.TrySetResult(result);
            };
            if (!field.Rotate((int)position.Direction) || !field.CanMoveTo(field.Object, position.Point)) tcs.TrySetCanceled();
            field.PlaceAt(position);

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