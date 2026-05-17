import { useState } from "react";
import planoraLogo from '../images/Planora-logo.png';
import { useNavigate } from "react-router-dom"; 
import api from "../api/axios"; 

export default function Login() {

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
   const navigate = useNavigate();

  
  const handleLogin = async () => {
  if (!email || !password) {
    alert("Please enter email and password!");
    return;
  }

  try {
    const response = await api.post("/Auth/login", {
      email: email,
      password: password
    });

    const data = response.data;

    console.log("Login success:", data);
    localStorage.setItem("token", data.accessToken);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("userId", data.userId);
    localStorage.setItem("email", data.email);
    navigate("/dashboard");

    setEmail("");
    setPassword("");

  } catch (error) {
    console.error("Error:", error);

    if (error.response) {
      alert(error.response.data?.message || "Login failed");
    } else {
      alert("Server error");
    }
  }
};

  return (
    <div className="flex min-h-screen">
    
      <div className="flex-1 bg-gradient-to-br from-[rgb(100,180,220)] via-[rgb(170,120,220)] to-[rgb(240,140,200)] p-20 flex flex-col justify-center">
        <h1 className="text-5xl font-bold text-indigo-900 mb-10">
          Welcome Back. <br /> Continue your journey.<br /> Achieve your goals.
        </h1>

        <div className="mb-8">
          <h3 className="font-semibold text-lg text-indigo-800">
            Goal-Based Planning
          </h3>
          <p className="text-gray-700">
            Log in to track your academic and work goals, monitor your progress automatically over time.
          </p>
        </div>

        <div className="mb-8">
          <h3 className="font-semibold text-lg text-indigo-800">
            Smart Productivity Analytics
          </h3>
          <p className="text-gray-700">
            Analyze your completed tasks, study sessions, and performance with interactive charts and real-time stats.
          </p>
        </div>

        <div className="fixed bottom-0 left-0 m-4">
          <img src={planoraLogo} alt="Planora" className="w-64 h-auto" />
        </div>
      </div>


      <div className="flex-1 flex items-center justify-center bg-gray-50">
        <div className="bg-white p-10 rounded-xl shadow-lg w-[380px]">

          <h2 className="text-2xl font-semibold mb-6">
            Log in to your account
          </h2>

          <div className="space-y-4">

            <input
              type="email"
              placeholder="Email address"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full border rounded-lg p-3 outline-none focus:ring-2 focus:ring-[#574cec]"
            />

            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full border rounded-lg p-3 outline-none focus:ring-2 focus:ring-[#574cec]"
            />

            <button
              onClick={handleLogin}
              className="w-full bg-[#7c78b8] hover:bg-[#5b588d] text-white p-3 rounded-lg font-medium"
            >
              Log In
            </button>

          </div>

          <div className="mt-6 text-center text-gray-500">
            or <a href="/register" className="text-[#574cec]  hover:underline">
              register
            </a>
          </div>

          

        </div>
      </div>
    </div>
  );
}