import { useEffect, useState } from "react";
import api from "../api/axios";
import { BsArrowDown } from "react-icons/bs";
import { BsArrowUp } from "react-icons/bs";
import { FcFullTrash } from "react-icons/fc";

export default function Subjects() {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openSubjectId, setOpenSubjectId] = useState(null);
  const [subjectDetails, setSubjectDetails] = useState({});

  const [showModal, setShowModal] = useState(false);
  const [newSubject, setNewSubject] = useState("");


  const userId = localStorage.getItem("userId");
  const token = localStorage.getItem("token");
  const toggleSubject = async (id) => {
    if (openSubjectId === id) {
      setOpenSubjectId(null);
      return;
    }

    setOpenSubjectId(id);

    if (subjectDetails[id]) return;
    

    try {
      const res = await api.get(`/Subject/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setSubjectDetails((prev) => ({
        ...prev,
        [id]: res.data,
      }));
    } catch (err) {
      console.error("Detail error:", err);
    }
  };
  const getSubjects = async () => {
    try {
      const res = await api.get(`/Subject/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setSubjects(res.data);
    } catch (err) {
      console.error(err);
      setSubjects([]);
    }
    setLoading(false);
  };

  useEffect(() => {
    getSubjects();
  }, []);
  const handleAddSubject = async () => {
      if (!newSubject.trim()) return;

      try {
        const res = await api.post(
          "/Subject",
          {
            name: newSubject,
          },
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        setSubjects([...subjects, res.data]);

        setNewSubject("");
        setShowModal(false);
      } catch (err) {
        console.error("Add subject error:", err);
      }
    };
    const handleDeleteSubject = async (subjectId, e) => {
      e.stopPropagation();
      if (!confirm("Are you sure you want to delete this subject?")) return;

      try {
        await api.delete(`/Subject/${subjectId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setSubjects(prev => prev.filter(s => s.id !== subjectId));
      } catch (err) {
        console.error("Delete subject error:", err.response?.data || err);
        alert("Failed to delete subject. Please try again.");
      }
    };

  return (
    <div className="p-6">
    <h1 className="text-3xl font-bold mb-4">Subjects</h1>

    <div className="bg-white shadow rounded-3xl p-4">
   
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold">Your Subjects</h2>
        <button
          onClick={() => setShowModal(true)}
          className="bg-[#7c78b8] text-white px-4 py-2 rounded hover:bg-green-600"
        >
          + Add Subject
        </button>
      </div>
      {loading ? (
        <p>Loading...</p>
      ) : subjects.length === 0 ? (
        <p>No subjects found.</p>
      ) : (
        <ul className="space-y-3">
          {subjects.map((subject) => {
            const detail = subjectDetails[subject.id];

            return (
              <li
                key={subject.id}
                className="bg-gray-100 rounded-lg relative p-6"
              >
           
                <div className="flex justify-between items-center">
                  <span className="font-medium text-lg">{subject.name}</span>

                  <div className="flex items-center gap-2">
            
                    <button
                      onClick={() => toggleSubject(subject.id)}
                      className="text-xl px-2 hover:text-gray-600"
                    >
                      {openSubjectId === subject.id ? (
                        <BsArrowUp />
                      ) : (
                        <BsArrowDown />
                      )}
                    </button>
                    <button
                      onClick={(e) => handleDeleteSubject(subject.id, e)}
                      className="text-red-500 hover:text-red-700 font-bold "
                    >
                      <FcFullTrash className="text-xl" />
                    </button>
                  </div>
                </div>

            
                {openSubjectId === subject.id && (
                  <div className="mt-4 px-2 text-sm">
                    {!detail ? (
                      <p>Loading...</p>
                    ) : (
                      <>
                        <p className="text-[#7c78b8] font-medium">
                          Study Goals: {detail.studyGoals?.length || 0}
                        </p>

                        {detail.studyGoals?.length > 0 && (
                          <ul className="mt-2 list-disc ml-5">
                            {detail.studyGoals.map((goal) => (
                              <li key={goal.id}>{goal.title}</li>
                            ))}
                          </ul>
                        )}
                      </>
                    )}
                  </div>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </div>

      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40">
          <div className="bg-white p-6 rounded shadow w-80">
            <h2 className="text-xl font-semibold mb-4">
              Add New Subject
            </h2>

            <input
              type="text"
              placeholder="Subject name..."
              value={newSubject}
              onChange={(e) => setNewSubject(e.target.value)}
              className="w-full border p-2 rounded mb-4"
            />

            <div className="flex justify-end gap-2">
              <button
                onClick={() => setShowModal(false)}
                className="px-3 py-1 bg-gray-300 rounded"
              >
                Cancel
              </button>

              <button
                onClick={handleAddSubject}
                className="px-3 py-1 bg-green-500 text-white rounded"
              >
                Add
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}