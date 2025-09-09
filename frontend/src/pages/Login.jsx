import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const VITE_API_BASE = import.meta.env.VITE_API_BASE;

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const submit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await fetch(`${VITE_API_BASE}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });
      if (!res.ok) {
        let msg = res.statusText;
        try {
          const j = await res.json();
          msg = j?.message || msg;
        } catch {}
        throw new Error(msg);
      }
      const data = await res.json();
      localStorage.setItem("access_token", data.accessToken);
      navigate("/");
    } catch (e) {
      alert(`Login gagal: ${e.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-[70vh] grid place-items-center p-4">
      <form
        onSubmit={submit}
        className="w-full max-w-sm bg-white p-6 rounded-2xl shadow space-y-3"
      >
        <h1 className="text-xl font-semibold">Login</h1>
        <div>
          <label className="text-sm">Email</label>
          <input
            className="w-full border rounded px-3 py-2"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label className="text-sm">Password</label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button
          disabled={loading}
          className="w-full rounded-xl px-3 py-2 bg-black text-white hover:opacity-90"
        >
          {loading ? "Loading..." : "Login"}
        </button>
      </form>
    </div>
  );
}
