using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using SSTUScheduleBot.CoreBase;

namespace SSTUScheduleBot.Schedule
{
    public class ScheduleParser
    {
        public List<Group> Parse()
        {
            var groups = ParseGroups().Result;

            Parallel.ForEach(groups, (g, s, i) =>
            {
                try
                {
                    g.Lessons = ParseLessons(g.Id).Result;
                }
                catch (Exception)
                {
                    Core.Debugger.Write("Exception on ", i);
                }

                Core.Debugger.Write($"[{i + 1}/{groups.Count}] parsed");
            });

            return groups;
        }

        public async Task<Dictionary<WeekTypes, List<Lesson>>> ParseLessons(int id)
        {
            var config   = Configuration.Default.WithDefaultLoader();
            var address  = $"http://rasp.sstu.ru/group/{id}";
            var context  = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);

            Dictionary<WeekTypes, List<Lesson>> result = new Dictionary<WeekTypes, List<Lesson>>();

            result[WeekTypes.Odd]  = ParseWeek(document.QuerySelector(".nechet"));
            result[WeekTypes.Even] = ParseWeek(document.QuerySelector(".chet"));

            return result;
        }

        private List<Lesson> ParseWeek(IElement document)
        {
            List<Lesson> schedule = new List<Lesson>();
            foreach (var column in document.QuerySelectorAll(".rasp-table-col"))
            {
                var day = column.QuerySelectorAll(".rasp-table-row-header > .rasp-table-inner-cell").First().Text()
                    .Trim();

                var pares = column.QuerySelectorAll(".rasp-table-row");
                foreach (var pare in pares)
                {
                    if (pare.ClassName.Contains("empty")) continue;

                    Lesson lesson = new Lesson();

                    var hidden = pare.QuerySelector(".rasp-table-inner-cell-hidden").TextContent.Split('\n');

                    lesson.Order =
                        int.Parse(hidden[1]
                            .Trim()) - 1;

                    if (pare.QuerySelectorAll(".subject").Length == 0)
                    {
                        lesson.Name      = pare.QuerySelector(".subject-m").TextContent;
                        lesson.Subgroups = new List<Lesson>();

                        foreach (var subgroup in pare.QuerySelectorAll(".subgroup-info"))
                        {
                            Lesson subgroupLesson = new Lesson();

                            subgroupLesson.Name        = subgroup.QuerySelector(".subgroup").TextContent;
                            subgroupLesson.Teacher     = subgroup.QuerySelector(".teacher").TextContent;
                            subgroupLesson.LectureHall = subgroup.QuerySelector(".aud").TextContent;

                            lesson.Subgroups.Add(subgroupLesson);
                        }
                    }
                    else
                    {
                        lesson.LectureHall = pare.QuerySelector(".aud").TextContent;
                        lesson.Name        = pare.QuerySelector(".subject").TextContent;
                        lesson.Type        = pare.QuerySelector(".type").TextContent;
                        lesson.Teacher     = pare.QuerySelector(".teacher").TextContent;
                    }

                    lesson.Day = GetDay(day);

                    schedule.Add(lesson);
                }
            }

            return schedule;
        }

        private int GetDay(string day)
        {
            var dayClear = day[0] + day[1].ToString();

            return dayClear switch
            {
                "Пн" => 0,
                "Вт" => 1,
                "Ср" => 2,
                "Чт" => 3,
                "Пт" => 4,
                "Сб" => 5,
                _    => throw new ArgumentException(dayClear)
            };
        }

        private async Task<List<Group>> ParseGroups()
        {
            var config       = Configuration.Default.WithDefaultLoader();
            var address      = "http://rasp.sstu.ru/";
            var context      = BrowsingContext.New(config);
            var document     = await context.OpenAsync(address);
            var cellSelector = "div.col-group a";
            var cells        = document.QuerySelectorAll(cellSelector);
            var titles = cells.Select(m => new Group()
            {
                Name = m.TextContent,
                Id   = int.Parse(((IHtmlAnchorElement) m).Href.Split('/').Last())
            });

            return titles.ToList();
        }
    }
}