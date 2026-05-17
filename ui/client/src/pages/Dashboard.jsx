import { useEffect, useState } from "react";
import MyCalendar1 from "../components/MyCalendar1";
import StatsCardTask from "../components/StartsCardTask";
import GoalsProgress from "../components/GoalsProgress";
import api from "../api/axios";

export default function Dashboard() {
  const [date, setDate] = useState(new Date());
  const [tasks, setTasks] = useState([]);
  const [statusData, setStatusData] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [selectedSubjectId, setSelectedSubjectId] = useState("");
  const [loading, setLoading] = useState(true);
  const selectedSubject = subjects.find(
    (s) => String(s.id) === String(selectedSubjectId)
  );

  const userId = localStorage.getItem("userId");
  const token = localStorage.getItem("token");

  useEffect(() => {
    async function fetchData() {
      try {
 
        let resSubjects;
        try {
          resSubjects = await api.get(`/Subject/user/${userId}`, {
            headers: { Authorization: `Bearer ${token}` },
          });
        } catch {
          resSubjects = await api.post(
            `/Subject/user/${userId}`,
            {},
            { headers: { Authorization: `Bearer ${token}` } }
          );
        }

        const subjectList = Array.isArray(resSubjects.data)
          ? resSubjects.data
          : resSubjects.data.subjects || [];
        setSubjects(subjectList);
        if (subjectList.length > 0) setSelectedSubjectId(subjectList[0].id);

        
        const resTasks = await api.get(`/Task/user/${userId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        const taskArray = Array.isArray(resTasks.data)
          ? resTasks.data
          : resTasks.data.tasks || [];
        setTasks(taskArray);

        const pendingCount = taskArray.filter((t) => t.status === 0).length;
        const inProgressCount = taskArray.filter((t) => t.status === 1).length;
        const doneCount = taskArray.filter((t) => t.status === 2).length;

        setStatusData([
          { name: "Pending", value: pendingCount, color: "rgb(100,180,220)" },
          { name: "In Progress", value: inProgressCount, color: "rgb(59,130,246)" },
          { name: "Done", value: doneCount, color: "rgb(22,163,74)" },
        ]);

        setLoading(false);
      } catch (err) {
        console.error("Dashboard fetch error:", err);
        setTasks([]);
        setStatusData([]);
        setSubjects([]);
        setLoading(false);
      }
    }
    fetchData();
  }, [userId, token]);

  const filteredTasks = tasks.filter((task) => {
    const taskDate = new Date(task.deadline);
    return (
      taskDate.getFullYear() === date.getFullYear() &&
      taskDate.getMonth() === date.getMonth() &&
      taskDate.getDate() === date.getDate()
    );
  });

  const getStatus = (status) => {
    switch (status) {
      case 0:
        return "Pending";
      case 1:
        return "In Progress";
      case 2:
        return "Done";
      default:
        return "Unknown";
    }
  };

  return (
      <div className="flex gap-6 p-6 ">
    
      <div className=" max-w-[380px] bg-white p-4 rounded-3xl shadow-lg flex-shrink-0 flex flex-col">
        <h2 className="text-2xl font-bold text-gray-700 mb-4">Calendar</h2>
        <div className="flex-1">
          <MyCalendar1 date={date} setDate={setDate} tasks={tasks} />
        </div>
      </div>
      <div className="w-3/4 flex flex-col gap-6 h-full">
       
        <div className="flex gap-6 flex-1">
   
          <div className="bg-white p-6 rounded-3xl shadow-lg border border-gray-100 flex-1 h-full flex flex-col">
            <h2 className="text-2xl font-semibold text-gray-800 mb-4"> Goals Progress</h2>
            <div className="mb-4">
              <label className="block text-sm text-gray-500 mb-2">Select Subject</label>
              <div className="relative">
                <select
                  value={selectedSubjectId}
                  onChange={(e) => setSelectedSubjectId(e.target.value)}
                  className="w-full px-4 py-3 rounded-xl bg-gray-50 border border-gray-200 focus:outline-none focus:ring-2 focus:ring-[#7c78b8] hover:border-[#7c78b8] transition shadow-sm"
                >
                  {subjects.map((s) => (
                    <option key={s.id} value={s.id}>{s.name}</option>
                  ))}
                </select>
                <span className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400">▼</span>
              </div>
            </div>
            {!loading && <GoalsProgress subject={selectedSubject} />}
            {loading && <p>Loading goals...</p>}
          </div>

         
          <div className="bg-white p-6 pb-28 rounded-3xl shadow-lg border border-gray-100 flex-1 h-full flex flex-col justify-center">
            <h2 className="text-2xl font-semibold text-gray-800 mb-4"> Task Progress</h2>
            {!loading && <StatsCardTask data={statusData} />}
            {loading && <p>Loading tasks...</p>}
          </div>
        </div>


        <div className="bg-white p-6 rounded-3xl shadow-lg border border-gray-100 flex-1 flex flex-col">
          <h2 className="text-2xl font-semibold text-gray-800 mb-4"> Today's Tasks</h2>
          <ul className="space-y-2 flex-1">
            {filteredTasks.length > 0 ? (
              filteredTasks.map((task) => (
                <li
                  key={task.id}
                  className="bg-gradient-to-r from-white via-gray-100 to-white p-3 rounded-xl shadow hover:shadow-md transition flex justify-between items-center"
                >
                  <div>
                    <p className="font-medium">{task.title}</p>
                    {task.description && (
                      <p className="text-sm text-gray-500">{task.description}</p>
                    )}
                  </div>
                  <span
                    className={`px-3 py-1 rounded-full text-white text-xs font-semibold ${
                      task.status === 0
                        ? "bg-[rgb(100,180,220)]"
                        : task.status === 1
                        ? "bg-[rgb(170,120,220)]"
                        : "bg-[rgb(240,140,200)]"
                    }`}
                  >
                    {getStatus(task.status)}
                  </span>
                </li>
              ))
            ) : (
              <li className="text-gray-400 italic">No tasks for this day</li>
            )}
          </ul>
        </div>
      </div>
    </div>
  );
}