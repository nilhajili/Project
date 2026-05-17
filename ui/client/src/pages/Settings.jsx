import { useEffect, useState } from "react";
import api from "../api/axios";

export default function User() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const [editing, setEditing] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);

  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const userId = localStorage.getItem("userId");
  useEffect(() => {
    if (!userId) return;

    const fetchUser = async () => {
      setLoading(true);
      try {
        const res = await api.get(`/users/${userId}`);
        const userData = res.data;

        setUser(userData);
        setFullName(userData.fullName);
        setEmail(userData.email);
      } catch (err) {
        console.error("User fetch error:", err);
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, [userId]);
  const handleUpdate = async () => {
    try {
      const res = await api.put(`/users/${userId}`, {
        fullName,
        email,
      });

      setUser(res.data);
      setEditing(false);
    } catch (err) {
      console.error("Update error:", err);
      alert("Update failed - maybe email is taken?");
    }
  };
  const handleChangePassword = async () => {
    if (!currentPassword || !newPassword) {
      alert("Fill all fields please");
      return;
    }

    try {
      await api.put(`/users/${userId}/change-password`, {
        currentPassword,
        newPassword,
      });

      alert("Password updated ");
      setChangingPassword(false);
      setCurrentPassword("");
      setNewPassword("");
    } catch (err) {
      console.error(err);
      alert("Current password is wrong ");
    }
  };
  const handleDelete = async () => {
    if (!window.confirm("Are you sure?")) return;

    try {
      await api.delete(`/users/${userId}`);
      localStorage.clear();
      window.location.href = "/login";
    } catch (err) {
      console.error("Delete error:", err);
    }
  };

  if (loading) return <p className="p-6">Loading profile...</p>;
  if (!user) return <p className="p-6 text-red-500">User not found</p>;

  return (
    <div className="p-6 flex flex-col gap-6">
      <div className="bg-white p-6 rounded-3xl shadow-lg border border-gray-100 max-w">
        <h2 className="text-2xl font-semibold text-gray-800 mb-4">
          Profile Info
        </h2>

        {editing ? (
          <div className="flex flex-col gap-3">
            <input
              className="p-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-[#7c78b8]"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Full Name"
            />
            <input
              className="p-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-[#7c78b8]"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Email"
            />

            <button
              onClick={handleUpdate}
              className="bg-green-500 text-white py-2 rounded-xl"
            >
              Save Changes
            </button>
          </div>
        ) : (
          <div className="flex flex-col gap-2">
            <p><span className="font-semibold">Name:</span> {user.fullName}</p>
            <p><span className="font-semibold">Email:</span> {user.email}</p>
          </div>
        )}
      </div>
      <div className="bg-white p-6 rounded-3xl shadow-lg border border-gray-100 max-w">
        <h2 className="text-xl font-semibold text-gray-800 mb-4">
          Change Password
        </h2>

        {changingPassword ? (
          <div className="flex flex-col gap-3">
            <input
              type="password"
              placeholder="Current Password"
              className="p-3 rounded-xl border border-gray-200"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
            />

            <input
              type="password"
              placeholder="New Password"
              className="p-3 rounded-xl border border-gray-200"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />

            <button
              onClick={handleChangePassword}
              className="bg-blue-500 text-white py-2 rounded-xl"
            >
              Update Password
            </button>

            <button
              onClick={() => setChangingPassword(false)}
              className="bg-gray-300 py-2 rounded-xl"
            >
              Cancel
            </button>
          </div>
        ) : (
          <button
            onClick={() => setChangingPassword(true)}
            className="bg-[#7c78b8] text-white py-2 rounded-xl w-full"
          >
            Change Password
          </button>
        )}
      </div>

      <div className="bg-white p-6 rounded-3xl shadow-lg border border-gray-100 max-w flex flex-col gap-3">
        <h2 className="text-xl font-semibold text-gray-800">Actions</h2>

        <button
          onClick={() => setEditing(!editing)}
          className="bg-[#7c78b8] text-white py-2 rounded-xl"
        >
          {editing ? "Cancel" : "Edit Profile"}
        </button>

        <button
          onClick={handleDelete}
          className=" bg-[#7c78b8] hover:bg-red-500 text-white py-2 rounded-xl"
        >
          Delete Account
        </button>
      </div>
    </div>
  );
}