import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";

function PaymentSuccessPage() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [params] = useSearchParams();

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  const sessionId = params.get("session_id");

  return (
    <main style={{ width: "100%", maxWidth: "920px", margin: "0 auto", padding: "24px" }}>
      <section style={{ border: "1px solid #e5e7eb", borderRadius: "12px", padding: "18px" }}>
        <h1 style={{ margin: "0 0 8px", fontSize: "34px" }}>Placanje uspesno</h1>
        <p style={{ margin: 0, color: "#374151" }}>
          Stripe je potvrdio uplatu. Racun ce uskoro biti oznacen kao placen u sistemu.
        </p>
        {sessionId && (
          <p style={{ marginTop: "10px", color: "#6b7280", fontSize: "13px" }}>
            Session: <span style={{ fontFamily: "monospace" }}>{sessionId}</span>
          </p>
        )}

        <div style={{ display: "flex", gap: "10px", marginTop: "14px" }}>
          <button
            onClick={() => navigate("/monthly-billing")}
            style={{
              backgroundColor: "var(--secondary)",
              color: "var(--white)",
              border: "none",
              borderRadius: "10px",
              padding: "10px 14px",
              fontWeight: 700,
              cursor: "pointer"
            }}
          >
            Nazad na racune
          </button>
          <button
            onClick={() => navigate("/")}
            style={{
              background: "#eef2ff",
              border: "1px solid #dbeafe",
              borderRadius: "10px",
              padding: "10px 14px",
              cursor: "pointer",
              fontWeight: 700
            }}
          >
            Dashboard
          </button>
        </div>
      </section>
    </main>
  );
}

export default PaymentSuccessPage;

