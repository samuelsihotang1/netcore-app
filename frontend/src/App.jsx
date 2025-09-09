import { Routes, Route, Navigate, Link } from "react-router-dom";
import Products from "./pages/Products.jsx";
import Login from "./pages/Login.jsx";
import Register from "./pages/Register.jsx";
import Orders from "./pages/Orders.jsx";
import OrderDetail from "./pages/OrderDetail.jsx";

function isAuthed() {
  return !!localStorage.getItem("access_token");
}

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <header className="border-b bg-white">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
          <Link to="/products" className="text-xl font-semibold">Shop</Link>
          <nav className="flex items-center gap-2">
            <Link to="/orders" className="px-3 py-1 rounded hover:bg-gray-100">Orders</Link>
            {!isAuthed() ? (
              <>
                <Link to="/login" className="px-3 py-1 rounded hover:bg-gray-100">Login</Link>
                <Link to="/register" className="px-3 py-1 rounded hover:bg-gray-100">Register</Link>
              </>
            ) : (
              <button
                onClick={() => { localStorage.removeItem("access_token"); window.location.hash = "#/login"; }}
                className="px-3 py-1 rounded hover:bg-red-50 text-red-600 border border-red-200"
              >
                Logout
              </button>
            )}
          </nav>
        </div>
      </header>

      <main className="max-w-6xl mx-auto p-4">
        <Routes>
          <Route path="/" element={<Products />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/orders" element={<Orders />} />
          <Route path="/orders/:id" element={<OrderDetail />} />
          <Route path="*" element={<div>404 Not Found</div>} />
        </Routes>
      </main>
    </div>
  );
}
