import { useEffect, useState } from "react";
import api from "../api/axios";
import MyCalendar from "../components/MyCalendar1";
import { FcFullTrash } from "react-icons/fc";

export const TaskStatus = { Pending: 0, InProgress: 1, Done: 2 };
export const TaskStatusLabel = { 0: "Pending", 1: "In Progress", 2: "Done" };
export const TaskStatusColor = { 0: "text-[rgb(100,180,220)]", 1: "text-[rgb(170,120,220)]", 2: "text-[rgb(240,140,200)]" };

export default function TasksDashboard() {
  const [tasks, setTasks] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [date, setDate] = useState(new Date());

  const [showModal, setShowModal] = useState(false);
  const [modalType, setModalType] = useState("add");
  const [currentTask, setCurrentTask] = useState({
    name: "",
    description: "",
    deadline: "",
    status: TaskStatus.Pending,
    subjectId: "",
  });

  const userId = localStorage.getItem("userId");
  const token = localStorage.getItem("token");

  const fetchTasks = async () => {
    setLoading(true);
    try {
      const res = await api.get(`/Task/user/${userId}`);
      setTasks(res.data);
    } catch (err) {
      console.error(err);
    }
    setLoading(false);
  };

  const fetchSubjects = async () => {
    try {
      const res = await api.get(`/Subject/user/${userId}`);
      setSubjects(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchTasks();
    fetchSubjects();
  }, []);

  
  const handleAddClick = () => {
    setModalType("add");
    setCurrentTask({
      name: "",
      description: "",
      deadline: "",
      status: TaskStatus.Pending,
      subjectId: "",
    });
    setShowModal(true);
  };

  const handleEditClick = (task) => {
    setModalType("update");
    setCurrentTask({
      ...task,
      deadline: task.deadline ? task.deadline.slice(0, 10) : "",
    });
    setShowModal(true);
  };

  const handleAddTask = async () => {
    if (!currentTask.name.trim()) return;
    try {
      const res = await api.post(
        "/Task",
        { ...currentTask, userId },
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setTasks([...tasks, res.data]);
      setShowModal(false);
    } catch (err) {
      console.error("Add task error:", err.response?.data || err);
    }
  };

  const handleUpdateTask = async () => {
    if (!currentTask.name.trim()) return;
    try {
      const res = await api.put(
        `/Task/${currentTask.id}`,
        { ...currentTask, userId },
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setTasks(tasks.map((t) => (t.id === res.data.id ? res.data : t)));
      setShowModal(false);
    } catch (err) {
      console.error("Update task error:", err.response?.data || err);
    }
  };


  const handleDeleteTask = async (taskId, e) => {
      e.stopPropagation();
      if (!confirm("Are you sure you want to delete this task?")) return;

      try {
        await api.delete(`/Task/${taskId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setTasks(prev => prev.filter(t => t.id !== taskId));
      } catch (err) {
        console.error("Delete task error:", err.response?.data || err);
        alert("Failed to delete task. Please try again.");
      }
    };

  return (
    <div className="flex gap-6 p-6 h-full">
   
      <div className="w-1/3 rounded-3xl p-4 shadow">
        <MyCalendar date={date} setDate={setDate} />
      </div>

    
      <div className="flex-1 flex flex-col gap-4">
        <div className="flex justify-between items-center">
          <h1 className="text-3xl  text-black font-bold">Tasks</h1>
          <button
            onClick={handleAddClick}
            className="bg-[#7c78b8] text-white px-4 py-2 rounded hover:bg-green-600"
          >
            + Add Task
          </button>
        </div>

        {loading ? (
          <p>Loading tasks...</p>
        ) : tasks.length === 0 ? (
          <p>No tasks available</p>
        ) : (
          <div className="flex flex-col gap-3 overflow-y-auto max-h-[600px]">
            {tasks.map((task) => (
              <div
                  key={task.id}
                  onClick={() => handleEditClick(task)}
                  className="group relative cursor-pointer p-4 rounded-2xl shadow bg-white transition transform hover:bg-[#7c78b8] hover:shadow-3xl  hover:-translate-y-1 hover:scale-30"
                >
                  
                 <button
                        onClick={(e) => handleDeleteTask(task.id, e)}
                        className="absolute top-2 right-2 text-red-500 hover:text-red-700 font-bold"
                      >
                        <FcFullTrash className="text-3xl" />
                  </button>
                  <div>
                    <h2 className="font-semibold text-black group-hover:text-white">
                        Name: {task.name}
                    </h2>

                    <p className="text-black group-hover:text-white">
                      Description: {task.description}
                    </p>

                    <p className="text-sm text-black group-hover:text-white">
                      Deadline: {task.deadline ? new Date(task.deadline).toLocaleDateString() : "No deadline"}
                    </p>

                    <p className="text-sm text-black group-hover:text-white">
                      Subject: {task.subjectId ? subjects.find((s) => s.id === task.subjectId)?.name : "None"}
                    </p>

                    <p className={`text-sm ${TaskStatusColor[task.status]} font-semibold group-hover:text-white`}>
                      Status: {TaskStatusLabel[task.status]}
                    </p>
                  </div>
                </div>
            ))}
          </div>
        )}
      </div>

  
      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40 z-50">
          <div className="bg-white p-6 rounded shadow w-96">
            <h2 className="text-xl font-semibold mb-4">
              {modalType === "add" ? "Add New Task" : "Update Task"}
            </h2>

            <input
              type="text"
              placeholder="Task Name"
              value={currentTask.name}
              onChange={(e) => setCurrentTask({ ...currentTask, name: e.target.value })}
              className="w-full border p-2 rounded mb-2"
            />
            <textarea
              placeholder="Description"
              value={currentTask.description}
              onChange={(e) => setCurrentTask({ ...currentTask, description: e.target.value })}
              className="w-full border p-2 rounded mb-2"
            />
            <input
              type="date"
              value={currentTask.deadline}
              onChange={(e) => setCurrentTask({ ...currentTask, deadline: e.target.value })}
              className="w-full border p-2 rounded mb-2"
            />
            <select
              value={currentTask.subjectId || ""}
              onChange={(e) => setCurrentTask({ ...currentTask, subjectId: e.target.value })}
              className="w-full border p-2 rounded mb-2"
            >
              <option value="">Select Subject</option>
              {subjects.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
            <select
              value={currentTask.status}
              onChange={(e) => setCurrentTask({ ...currentTask, status: Number(e.target.value) })}
              className="w-full border p-2 rounded mb-2"
            >
              <option value={TaskStatus.Pending}>Pending</option>
              <option value={TaskStatus.InProgress}>In Progress</option>
              <option value={TaskStatus.Done}>Done</option>
            </select>

            <div className="flex justify-end gap-2 mt-4">
              <button onClick={() => setShowModal(false)} className="px-3 py-1 bg-gray-300 rounded">
                Cancel
              </button>
              <button
                onClick={modalType === "add" ? handleAddTask : handleUpdateTask}
                className={`px-3 py-1 rounded text-white ${
                  modalType === "add" ? "bg-green-500" : "bg-blue-500"
                }`}
              >
                {modalType === "add" ? "Add" : "Update"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}