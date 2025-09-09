import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";

const VITE_API_BASE = import.meta.env.VITE_API_BASE;
const getTok = () => localStorage.getItem("access_token") || "";
const ORDER_STATUS_OPTIONS = ["paid", "processing", "completed", "cancelled"];

export default function Orders() {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState(null);
  const navigate = useNavigate();

  const fetchOrders = async () => {
    try {
      const token = getTok();
      if (!token) {
        navigate("/login");
        return;
      }
      const res = await fetch(`${VITE_API_BASE}/orders`, {
        headers: { Authorization: `Bearer ${token}` },
        cache: "no-store",
      });
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      setRows(await res.json());
    } catch (e) {
      setErr(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const updateOrderStatus = async (id, status) => {
    try {
      const token = getTok();
      const res = await fetch(`${VITE_API_BASE}/orders/${id}/status`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ status }),
      });
      if (!res.ok) {
        let msg = res.statusText;
        try {
          const j = await res.json();
          msg = j?.message || msg;
        } catch {}
        throw new Error(msg);
      }
      await fetchOrders();
    } catch (e) {
      alert(`Gagal update: ${e.message}`);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (err) return <div className="text-red-600">{err}</div>;

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Order Saya</h1>
        <Link
          to="/products"
          className="px-3 py-1 rounded border hover:bg-gray-50"
        >
          ← Kembali
        </Link>
      </div>

      <div className="overflow-auto border rounded-2xl">
        <table className="min-w-full text-sm">
          <thead className="bg-gray-100 text-gray-700">
            <tr>
              <th className="p-3 text-left">ID</th>
              <th className="p-3 text-left">Produk</th>
              <th className="p-3 text-left">Qty</th>
              <th className="p-3 text-left">Harga</th>
              <th className="p-3 text-left">Subtotal</th>
              <th className="p-3 text-left">Shipping</th>
              <th className="p-3 text-left">Grand</th>
              <th className="p-3 text-left">Status Order</th>
              <th className="p-3 text-left">Delivery</th>
              <th className="p-3 text-left">Aksi</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.id} className="border-t">
                <td className="p-3">#{r.id}</td>
                <td className="p-3">{r.productName}</td>
                <td className="p-3">{r.qty}</td>
                <td className="p-3">
                  Rp {Number(r.unitPrice).toLocaleString("id-ID")}
                </td>
                <td className="p-3">
                  Rp {Number(r.subtotal).toLocaleString("id-ID")}
                </td>
                <td className="p-3">
                  Rp {Number(r.shippingCost).toLocaleString("id-ID")}
                </td>
                <td className="p-3">
                  Rp {Number(r.grandTotal).toLocaleString("id-ID")}
                </td>
                <td className="p-3">
                  <select
                    value={r.status}
                    onChange={(e) => updateOrderStatus(r.id, e.target.value)}
                    className="border rounded px-2 py-1"
                  >
                    {ORDER_STATUS_OPTIONS.map((x) => (
                      <option key={x} value={x}>
                        {x}
                      </option>
                    ))}
                  </select>
                </td>
                <td className="p-3">
                  {r.shipmentStatus ?? <span className="text-gray-500">-</span>}
                </td>
                <td className="p-3">
                  <Link to={`/orders/${r.id}`} className="underline">
                    Detail
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
