import { useEffect, useState } from "react";
import api from "../api/axios";
import { RiEditLine } from "react-icons/ri";
import { FcFullTrash } from "react-icons/fc";

export default function Goals() {
  const [subjects, setSubjects] = useState([]);
  const [selectedSubjectId, setSelectedSubjectId] = useState(null);
  const [goals, setGoals] = useState([]);
  const [loadingSubjects, setLoadingSubjects] = useState(true);
  const [loadingGoals, setLoadingGoals] = useState(false);
  const [goalModal, setGoalModal] = useState({ open: false, goal: null });
  const [newGoalTitle, setNewGoalTitle] = useState("");
  const token = localStorage.getItem("token");
  const userId = localStorage.getItem("userId");
  const getSubjects = async () => {
    try {
      const res = await api.get(`/Subject/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setSubjects(res.data);
      if (res.data.length > 0) setSelectedSubjectId(res.data[0].id);
    } catch (err) {
      console.error("Failed to load subjects:", err);
    }
    setLoadingSubjects(false);
  };


  const getGoals = async (subjectId) => {
    if (!subjectId) return;
    setLoadingGoals(true);
    try {
      const res = await api.get(`/StudyGoal/${subjectId}/goals`, {
        headers: { Authorization: `Bearer ${token}` },
        });
      setGoals(res.data || []);
    } catch (err) {
      console.error("Failed to load goals:", err);
      setGoals([]);
    }
    setLoadingGoals(false);
  };

  const handleSubjectChange = (e) => {
    const subjectId = e.target.value;
    setSelectedSubjectId(subjectId);
    getGoals(subjectId);
  };


 const handleAddGoal = async () => {
    try {
        const res = await api.post(
        `/StudyGoal`,
        { 
            userId: userId,                 
            subjectId: selectedSubjectId,
            targetHours: 1                  
        },
        { headers: { Authorization: `Bearer ${token}` } }
        );
        setGoals([...goals, res.data]);
    } catch (err) {
        console.error("Failed to add goal:", err);
    }
    };


  const handleAddHours = async (goalId, hours = 1) => {
    try {
        await api.post(`/StudyGoal/${goalId}/add-hours?hours=1`, null, {
        headers: { Authorization: `Bearer ${token}` },
        });
        getGoals(selectedSubjectId);
    } catch (err) {
        console.error("Failed to add hours:", err);
    }
    };


  const handleDeleteGoal = async (goalId) => {
    if (!confirm("Delete this goal?")) return;
    try {
      await api.delete(`/StudyGoal/${selectedSubjectId}/goals/${goalId}`, null, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setGoals((prev) => prev.filter((g) => g.id !== goalId));
    } catch (err) {
      console.error("Failed to delete goal:", err);
    }
  };


  const handleUpdateGoal = async () => {
    try {
      await api.put(`/StudyGoal/${selectedSubjectId}/goals/${goalModal.goal.id}`, {
        title: goalModal.goal.title,
        targetHours: goalModal.goal.targetHours || 1
      }, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setGoalModal({ open: false, goal: null });
      getGoals(selectedSubjectId);
    } catch (err) {
      console.error("Failed to update goal:", err);
    }
  };

  useEffect(() => { getSubjects(); }, []);
  useEffect(() => { if (selectedSubjectId) getGoals(selectedSubjectId); }, [selectedSubjectId]);

return (
  <div className="p-6">
    <h1 className="text-3xl font-bold mb-4">Goals</h1>

    
    <div className="mb-6">
            {loadingSubjects ? (
                <p className="text-gray-400 animate-pulse">Loading ...</p>
            ) : (
                <div className="bg-white border border-gray-100 shadow-md rounded-2xl p-4">
                
                <p className="text-sm text-gray-500 mb-2 flex items-center gap-2">
                    Subject
                </p>

                <div className="relative">
                    <select
                    value={selectedSubjectId || ""}
                    onChange={handleSubjectChange}
                    className="w-full appearance-none px-4 py-3 rounded-xl 
                                bg-gray-50 border border-gray-200 
                                text-gray-700 font-medium
                                focus:outline-none focus:ring-2 focus:ring-[#7c78b8]
                                hover:border-[#7c78b8]
                                transition-all duration-200 cursor-pointer"
                    >
                    {subjects.length === 0 ? (
                        <option>No subjects found</option>
                    ) : (
                        subjects.map((subj) => (
                        <option key={subj.id} value={subj.id}>
                            {subj.name}
                        </option>
                        ))
                    )}
                    </select>

                    <div className="pointer-events-none absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 text-sm">
                    ▼
                    </div>
                </div>
                </div>
            )}
            </div>

   
    <div className="mb-4 flex gap-2">
      <button
        onClick={handleAddGoal}
        className="bg-[#7c78b8] px-4 py-2 text-white rounded"
      >
        + Add Goal
      </button>
    </div>


    <div className="bg-white shadow rounded p-4">
      {loadingGoals ? (
        <p>Loading goals</p>
      ) : goals.length === 0 ? (
        <p>No goals for this subject</p>
      ) : (
        <ul className="space-y-3">
          {goals.map((goal, index) => (
            <li key={goal.id} className="bg-gray-100 rounded-lg p-4">
              <div className="flex justify-between items-center">
                
                <span className="font-medium">Goal #{index + 1}</span>
                <div className="flex gap-2">
                  <button
                    onClick={() => handleAddHours(goal.id)}
                    className="bg-[#3b3681] px-2 py-1 text-white rounded text-xs"
                  >
                    +1h
                  </button>
                  <button
                    onClick={() => setGoalModal({ open: true, goal })}
                    className="bg-[#7c78b8] px-2 py-1 text-white rounded text-xs"
                  >
                    <RiEditLine className="text-ml" />
                  </button>
                  <button
                    onClick={() => handleDeleteGoal(goal.id)}
                    className=" px-2 py-1 text-white rounded text-xs"
                  >
                     <FcFullTrash className="text-xl" />
                  </button>
                </div>
              </div>

              
              <div className="bg-gray-300 h-2 rounded mt-2">
                    <div
                        className="bg-[#f51a63] h-2 rounded"
                        style={{
                        width: `${Math.min((goal.currentHours / goal.targetHours) * 100, 100)}%`
                        }}
                    ></div>
                    </div>
                    <p className="text-xs mt-1 text-gray-600">
                    {goal.currentHours || 0} / {goal.targetHours || 1} hours
                    </p>`
            </li>
          ))}
        </ul>
      )}
    </div>

    {goalModal.open && (
      <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40">
        <div className="bg-white p-6 rounded shadow w-80">
          <h2 className="text-xl font-semibold mb-4">Edit Goal</h2>
          <input
            type="number"
            value={goalModal.goal.targetHours || 1}
            onChange={(e) =>
              setGoalModal((prev) => ({
                ...prev,
                goal: { ...prev.goal, targetHours: parseInt(e.target.value) },
              }))
            }
            className="w-full border p-2 rounded mb-4"
            min={1}
          />
          <div className="flex justify-end gap-2">
            <button
              onClick={() => setGoalModal({ open: false, goal: null })}
              className="px-3 py-1 bg-gray-300 rounded"
            >
              Cancel
            </button>
            <button
              onClick={handleUpdateGoal}
              className="px-3 py-1 bg-blue-500 text-white rounded"
            >
              Save
            </button>
          </div>
        </div>
      </div>
    )}
  </div>
);
}