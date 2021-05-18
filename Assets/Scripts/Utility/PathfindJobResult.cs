using Unity.Collections;

namespace Utility
{
    public struct PathfindJobResult
    {
        public NativeArray<int> Costs;
        public NativeList<JobNode> OutNodes;

        public PathfindJobResult(NativeArray<int> costs, NativeList<JobNode> outNodes)
        {
            Costs = costs;
            OutNodes = outNodes;
        }
    }
}