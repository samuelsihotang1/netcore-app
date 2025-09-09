import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";

const VITE_API_BASE = import.meta.env.VITE_API_BASE;
const getTok = () => localStorage.getItem("access_token") || "";
const SHIPMENT_STATUS_OPTIONS = ["packaging", "in_transit", "delivered", "failed"];

export default function OrderDetail() {
  const { id } = useParams();
  const orderId = Number(id);
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState(null);
  const navigate = useNavigate();

  const fetchDetail = async () => {
    try {
      const token = getTok();
      if (!token) {
        navigate("/login");
        return;
      }
      const res = await fetch(`${VITE_API_BASE}/orders/${orderId}`, {
        headers: { Authorization: `Bearer ${token}` },
        cache: "no-store",
      });
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      setData(await res.json());
    } catch (e) {
      setErr(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDetail();
  }, [orderId]);

  const updateShipmentStatus = async (status) => {
    try {
      const token = getTok();
      const res = await fetch(`${VITE_API_BASE}/orders/${orderId}/shipment/status`, {
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
      await fetchDetail();
    } catch (e) {
      alert(`Gagal update delivery: ${e.message}`);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (err) return <div className="text-red-600">{err}</div>;
  if (!data) return <div>Tidak ada data</div>;

  const s = data.shipment;
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Order #{data.id}</h1>
        <Link
          to="/orders"
          className="px-3 py-1 rounded border hover:bg-gray-50"
        >
          ← Kembali
        </Link>
      </div>

      <div className="grid md:grid-cols-2 gap-4">
        <div className="bg-white rounded-2xl shadow p-4">
          <div className="font-semibold mb-2">Ringkasan</div>
          <div className="text-sm text-gray-700 space-y-1">
            <div>Produk: {data.productName}</div>
            <div>Qty: {data.qty}</div>
            <div>
              Harga Satuan: Rp {Number(data.unitPrice).toLocaleString("id-ID")}
            </div>
            <div>
              Subtotal: Rp {Number(data.subtotal).toLocaleString("id-ID")}
            </div>
            <div>
              Ongkir: Rp {Number(data.shippingCost).toLocaleString("id-ID")}
            </div>
            <div className="font-semibold">
              Grand Total: Rp {Number(data.grandTotal).toLocaleString("id-ID")}
            </div>
            <div>
              Status Order:
              <span className="ml-2 px-2 py-0.5 text-xs rounded-full border border-emerald-200 text-emerald-700">
                {data.status}
              </span>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-2xl shadow p-4">
          <div className="font-semibold mb-2">Delivery</div>
          {s ? (
            <div className="space-y-2 text-sm text-gray-700">
              <div>
                Status:
                <span className="ml-2 px-2 py-0.5 text-xs rounded-full border border-blue-200 text-blue-700">
                  {s.status}
                </span>
              </div>
              <div>Courier: {s.courier ?? "-"}</div>
              <div>
                Dikirim:{" "}
                {s.shippedAt ? new Date(s.shippedAt).toLocaleString() : "-"}
              </div>
              <div>
                Diterima:{" "}
                {s.deliveredAt ? new Date(s.deliveredAt).toLocaleString() : "-"}
              </div>

              <div className="pt-2">
                <label className="text-sm">Ubah Status Delivery</label>
                <select
                  className="border rounded px-2 py-1 ml-2"
                  value={s.status}
                  onChange={(e) => updateShipmentStatus(e.target.value)}
                >
                  {SHIPMENT_STATUS_OPTIONS.map((opt) => (
                    <option key={opt} value={opt}>
                      {opt}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          ) : (
            <div className="text-gray-500">Tidak ada delivery.</div>
          )}
        </div>
      </div>
    </div>
  );
}
