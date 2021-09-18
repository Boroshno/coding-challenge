using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<System.Guid, Dictionary<System.Guid, List<Shirt>>> _hashedShirts;

        private Dictionary<Size, int> _sizeCounter;
        private Dictionary<Color, int> _colorCounter;

        public SearchEngine(List<Shirt> shirts)
        {
            _hashedShirts = shirts
                .GroupBy(x => x.Color.Id)
                .ToDictionary(x => x.Key, x => x.GroupBy(z => z.Size.Id)
                    .ToDictionary(y => y.Key, y => y.ToList()));
        }


        public SearchResults Search(SearchOptions options)
        {
            List<Shirt> resultShirts = new List<Shirt>();
            _sizeCounter = InitSizeCounter();
            _colorCounter = InitColorCounter();

            foreach (var color in Color.All)
            {
                int shirtsInTheColor = 0;
                foreach (var size in Size.All)
                {
                    Dictionary<System.Guid, List<Shirt>> shirtsByColor;
                    if (_hashedShirts.TryGetValue(color.Id, out shirtsByColor))
                    {
                        List<Shirt> filteredShirts;
                        if (shirtsByColor.TryGetValue(size.Id, out filteredShirts))
                        {
                            if ((options.Colors.Count == 0 || options.Colors.Select(x => x.Id).Contains(color.Id))
                                && (options.Sizes.Count == 0 || options.Sizes.Select(x => x.Id).Contains(size.Id)))
                            {
                                resultShirts.AddRange(filteredShirts);

                                UpdateCounters(true, size, true, color, filteredShirts.Count);
                            }
                            else 
                            {
                                UpdateCounters(options.Colors.Count == 0, size, options.Sizes.Count == 0, color, filteredShirts.Count);
                            }
                        }
                    }
                }
            }

            return new SearchResults
            {
                Shirts = resultShirts,
                ColorCounts = _colorCounter.Select(x => new ColorCount() { Color = x.Key, Count = x.Value }).ToList(),
                SizeCounts = _sizeCounter.Select(x => new SizeCount() { Size = x.Key, Count = x.Value }).ToList()
            };
        }

        private Dictionary<Size, int> InitSizeCounter()
        {
            Dictionary<Size, int> dicResult = new Dictionary<Size, int>();
            foreach (var size in Size.All)
            {
                dicResult[size] = 0;
            }
            return dicResult;
        }

        private Dictionary<Color, int> InitColorCounter()
        {
            Dictionary<Color, int> dicResult = new Dictionary<Color, int>();
            foreach (var color in Color.All)
            {
                dicResult[color] = 0;
            }
            return dicResult;
        }

        private void UpdateCounters(bool updateSizeCounter, Size size, bool updateColorCounter, Color color, int withValue)
        {
            if (updateColorCounter)
            {
                _colorCounter.TryGetValue(color, out var currentColorCount);
                _colorCounter[color] = currentColorCount + withValue;
            }
            if (updateSizeCounter)
            {
                _sizeCounter.TryGetValue(size, out var currentSizeCount);
                _sizeCounter[size] = currentSizeCount + withValue;
            }
        }
    }
}