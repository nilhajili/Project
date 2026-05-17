import { useState, useEffect } from "react";
import Calendar from "react-calendar";
import "react-calendar/dist/Calendar.css";
import "./calendar.css";

export default function MyCalendar1() {
  const [date, setDate] = useState(new Date());
  const [tasks, setTasks] = useState([]);

  const userId = localStorage.getItem("userId");

  useEffect(() => {
    fetch(`http://localhost:5196/api/Task/user/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    })
      .then((res) => res.json())
      .then((data) => {
        console.log("TASKS:", data);
        setTasks(data);
      })
      .catch((err) => console.error(err));
  }, []);

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
    <div className="flex flex-col items-center justify-center rounded-3xl p-4 bg-[#7c78b8]">
      <h2 className="text-white mb-4">Selected day: {date.toDateString()}</h2>

      <Calendar onChange={setDate} value={date} />

      <div className="mt-4 w-full bg-white rounded-xl p-3">
        <h3 className="font-semibold mb-2">Tasks:</h3>

        {filteredTasks.length === 0 ? (
          <p>No tasks for this day</p>
        ) : (
          filteredTasks.map((task) => (
            <div key={task.id} className="border-b py-2 flex justify-between">
              <span>{task.name}</span>
              <span className="text-sm text-gray-500">
                {getStatus(task.status)}
              </span>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
