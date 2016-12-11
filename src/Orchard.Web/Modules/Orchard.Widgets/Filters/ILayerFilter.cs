using Orchard.Widgets.Models;

namespace Orchard.Widgets.Filters
{
    public interface ILayerFilter : IDependency
    {
        bool ApplyFilter(LayerPart layerPart);
    }

}