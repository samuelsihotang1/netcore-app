import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";

const VITE_API_BASE = import.meta.env.VITE_API_BASE;
const getToken = () => localStorage.getItem("access_token") || "";

export default function Products() {
  const [products, setProducts] = useState([]);
  const [qtyById, setQtyById] = useState({});
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      try {
        const res = await fetch(`${VITE_API_BASE}/products`, {
          cache: "no-store",
        });
        if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
        console.log(`${VITE_API_BASE}/products`);
        setProducts(await res.json());
      } catch (e) {
        setErr(e.message);
      } finally {
        setLoading(false);
      }
    })();
  }, []);
  const buy = async (p) => {
    try {
      const qty = qtyById[p.id] ?? 1;
      const token = getToken();

      if (!token) {
        navigate("/login");
        return;
      }

      const res = await fetch(`${VITE_API_BASE}/orders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ productId: p.id, qty }),
      });

      if (res.status === 401) {
        navigate("/login");
        return;
      }

      if (!res.ok) {
        let msg = res.statusText;
        try {
          const j = await res.json();
          msg = j?.message || j?.Message || msg;
        } catch {}
        throw new Error(msg);
      }

      navigate("/orders");
    } catch (e) {
      alert(`Gagal beli: ${e.message}`);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (err) return <div className="text-red-600">{err}</div>;

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">Produk</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {products.map((p) => (
          <div
            key={p.id}
            className="bg-white rounded-2xl shadow p-4 flex flex-col gap-3"
          >
            <div className="flex items-start justify-between">
              <div>
                <div className="font-semibold text-lg">{p.name}</div>
                <div className="text-sm text-gray-500">SKU: {p.sku}</div>
              </div>
              <span
                className={`px-2 py-0.5 text-xs rounded-full border ${
                  p.isActive
                    ? "border-green-200 text-green-700"
                    : "border-gray-200 text-gray-500"
                }`}
              >
                {p.isActive ? "Active" : "Inactive"}
              </span>
            </div>
            <div className="text-2xl font-bold">
              Rp {Number(p.price).toLocaleString("id-ID")}
            </div>
            <div className="text-sm text-gray-600">Stock: {p.stock}</div>
            <div className="flex items-center gap-2">
              <input
                type="number"
                min={1}
                value={qtyById[p.id] ?? 1}
                onChange={(e) =>
                  setQtyById((m) => ({ ...m, [p.id]: Number(e.target.value) }))
                }
                className="w-20 border rounded px-2 py-1"
              />
              <button
                onClick={() => buy(p)}
                className="rounded-xl px-3 py-2 bg-black text-white hover:opacity-90"
              >
                Buy
              </button>
            </div>
          </div>
        ))}
      </div>

      <div className="text-sm text-gray-600">
        Belum login?{" "}
        <Link className="underline" to="/login">
          Login
        </Link>{" "}
        atau{" "}
        <Link className="underline" to="/register">
          Register
        </Link>
      </div>
    </div>
  );
}
