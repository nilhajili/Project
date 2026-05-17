import { useState } from "react";
import { useNavigate } from "react-router-dom"; 
import planoraLogo from "../images/Planora-logo.png";

export default function Register() {
  const navigate = useNavigate();

  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  

  const handleRegister = async () => {
    if (!fullName || !email || !password ) {
      alert("Please fill all fields!");
      return;
    }

    try {
      const response = await fetch("http://localhost:5196/api/Auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          fullName: fullName,
          email: email,
          password: password,
          
        })
      });

      const data = await response.json();

      if (response.ok) {
        console.log("Register success:", data);
        alert("Registration successful!");

        if (data.token) {
          localStorage.setItem("token", data.token);
         
        }

        setFullName("");
        setEmail("");
        setPassword("");
   
        navigate("/login");

      } else {
        alert(data.message || "Register failed");
      }

    } catch (error) {
      console.error("Error:", error);
      alert("Server error");
    }
  };

  return (
    <div className="flex min-h-screen">
      <div className="flex-1 bg-gradient-to-br from-[rgb(100,180,220)] via-[rgb(170,120,220)] to-[rgb(240,140,200)] p-20 flex flex-col justify-center">

        <h1 className="text-5xl font-bold text-indigo-900 mb-10">
          Plan your future. <br /> Track your progress.<br /> Achieve your goals.
        </h1>

        <div className="mb-8">
          <h3 className="font-semibold text-lg text-indigo-800">
            Goal-Based Planning
          </h3>
          <p className="text-gray-700">
            Set clear academic and work goals with measurable targets and track your progress automatically over time.
          </p>
        </div>

        <div className="mb-8">
          <h3 className="font-semibold text-lg text-indigo-800">
           Smart Productivity Analytics
          </h3>
          <p className="text-gray-700">
            Analyze your study sessions, completed tasks, and performance through interactive charts and real-time statistics.
          </p>
        </div>

        <div className="fixed bottom-0 left-0 m-4">
          <img src={planoraLogo} alt="Planora" className="w-64 h-auto " />
        </div>
      </div>
      <div className="flex-1 flex items-center justify-center bg-gray-50">

        <div className="bg-white p-10 rounded-xl shadow-lg w-[380px]">

          <h2 className="text-2xl font-semibold mb-6">
            Create an account
          </h2>

          <div className="space-y-4">

            <input
              type="text"
              placeholder="Full name"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              className="w-full border rounded-lg p-3 outline-none focus:ring-2 focus:ring-[#574cec]"
            />

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
              onClick={handleRegister}
              className="w-full bg-[#7c78b8] hover:bg-[#5b588d] text-white p-3 rounded-lg font-medium"
            >
              Sign Up
            </button>

          </div>

          <div className="mt-6 text-center text-gray-500">
            or <a href="/login" className="text-[#574cec]  hover:underline">
              log in
            </a>
          </div>

         

        </div>

      </div>

    </div>
  )
}