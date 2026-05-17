import { LineChart, Line, ResponsiveContainer, Tooltip } from "recharts";

export default function StatsCard({ data = [] }) {

  const avg = data.length
    ? Math.round(
        data.reduce((sum, item) => sum + Number(item.value || 0), 0) /
          data.length
      )
    : 0;

  return (
    <div className="bg-white p-6 rounded-2xl shadow-md w-[400px] h-[300px]">

    
      <div className="flex justify-between items-start">
        <div>
          <h2 className="text-2xl font-bold text-gray-800">
            {avg}%
          </h2>
          <p className="text-gray-500 text-sm">Average progress</p>
        </div>

        <p className="text-sm text-gray-500">Goals</p>
      </div>

 
      <div className="h-40 mt-4">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={data}>
            <Tooltip />

            <Line
              type="monotone"
              dataKey="value"
              stroke="#6C63FF"
              strokeWidth={3}
              dot={{ r: 4 }}
              activeDot={{ r: 6 }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      <div className="flex justify-between text-xs text-gray-400 mt-2">
        {data.map((item, i) => (
          <span key={i}>{item.name}</span>
        ))}
      </div>

    </div>
  );
}