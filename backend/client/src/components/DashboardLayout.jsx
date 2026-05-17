import { useEffect, useState } from "react";
import Sidebar from "../components/Sidebar";
import { Outlet } from "react-router-dom";
import api from "../api/axios";

export default function DashboardLayout() {
  const [user, setUser] = useState(null);

  const userId = localStorage.getItem("userId");

  useEffect(() => {
    if (!userId) return;

    const fetchUser = async () => {
      try {
        const res = await api.get(`/users/${userId}`);
        setUser(res.data);
      } catch (err) {
        console.error("User fetch error:", err);
      }
    };

    fetchUser();
  }, [userId]);

  return (
    <div className="flex min-h-screen">
      
      <div className="w-64 bg-white p-4 shadow-md border-r">
        <Sidebar />
      </div>

      <div className="flex flex-col flex-1">
        
        <div className="w-full h-60 overflow-hidden">
          <div className="w-full h-full bg-gradient-to-r from-[#7c78b8] to-[#e7dff3] flex items-center justify-center">
            
            <h1 className="text-4xl font-bold text-white">
              Welcome {user ? user.fullName : "..." } to Planora
            </h1>

          </div>
        </div>

        <div className="flex-1 p-6 bg-[#e7dff3]">
          <Outlet />
        </div>

      </div>
    </div>
  );
}