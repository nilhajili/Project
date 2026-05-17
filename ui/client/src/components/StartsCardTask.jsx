import { PieChart, Pie, Cell, ResponsiveContainer } from "recharts";

const COLORS = ["rgb(100,180,220)", "rgb(170,120,220)", "rgb(240,140,200)"]; 


export default function StartsCardTask({ data }) {
  return (
    <div className="bg-white p-6 rounded-2xl shadow-md w-[400px] h-[300px]">
        <h2 className="text-lg font-semibold mb-4">Task statistic</h2>
      <div className="flex justify-between mb-4 text-sm">
        <div className="flex items-center gap-2">
          <span className="w-3 h-3 bg-[rgb(100,180,220)] rounded-full"></span>
          <p>Pending</p>
        </div>

        <div className="flex items-center gap-2">
          <span className="w-3 h-3 bg-[rgb(170,120,220)] rounded-full"></span>
          <p>InProgress</p>
        </div>

        <div className="flex items-center gap-2">
          <span className="w-3 h-3 bg-[rgb(240,140,200)] rounded-full"></span>
          <p>Done</p>
        </div>
      </div>

    
      <div className="h-40">
        <ResponsiveContainer>
          <PieChart>
            <Pie
              data={data}
              dataKey="value"
              innerRadius={55}
              outerRadius={80}
              paddingAngle={5}
            >
              {data.map((entry, index) => (
                <Cell key={index} fill={COLORS[index]} />
              ))}
            </Pie>
          </PieChart>
        </ResponsiveContainer>
      </div>

    </div>
  );
}