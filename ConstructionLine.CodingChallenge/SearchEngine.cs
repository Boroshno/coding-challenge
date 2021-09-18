﻿using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<System.Guid, Dictionary<System.Guid, List<Shirt>>> _hashedShirts;

        Dictionary<Size, int> _sizeCounter;
        Dictionary<Color, int> _colorCounter;

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

            foreach (var color in (options.Colors.Count > 0 ? options.Colors : Color.All))
            {
                foreach (var size in (options.Sizes.Count > 0 ? options.Sizes : Size.All))
                {
                    Dictionary<System.Guid, List<Shirt>> shirtsByColor;
                    if (_hashedShirts.TryGetValue(color.Id, out shirtsByColor))
                    {
                        List<Shirt> filteredShirts;
                        if (shirtsByColor.TryGetValue(size.Id, out filteredShirts))
                        {
                            resultShirts.AddRange(filteredShirts);
                            UpdateCounters(color, size, filteredShirts.Count);
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

        private void UpdateCounters(Color color, Size size, int value)
        {
            _sizeCounter.TryGetValue(size, out var currentSizeCount);
            _sizeCounter[size] = currentSizeCount + value;

            _colorCounter.TryGetValue(color, out var currentColorCount);
            _colorCounter[color] = currentColorCount + value;
        }
    }
}