import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { RemoveValueByKey } from "../../helpers/local_storage";

type ProtectedRouteProps = {
  children: React.ReactNode;
  requiredRole?: string | string[]; 
  redirectTo?: string;
};

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  requiredRole,
  redirectTo = "/login",
}) => {
  const { isAuthenticated, user, isLoading, logout } = useAuth();
  const location = useLocation();

  const handleLogout = () => {
    RemoveValueByKey("jwt");
    logout();
  };

  if (isLoading) {
    return <h1>Loading...</h1>;
  }

  if (!isAuthenticated) {
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  if (requiredRole) {
    const rolesAllowed = Array.isArray(requiredRole) ? requiredRole : [requiredRole];
    if (!rolesAllowed.includes(user?.role ?? "")) {
      return (
        <main
          className="min-h-screen flex items-center justify-center font-inter"
          style={{
            background: `linear-gradient(135deg, #C3FDB8 0%, #FFF8C6 50%, #BDEDFF 100%)`,
          }}
        >
          <div
            className="shadow-2xl rounded-2xl px-10 py-14 text-center max-w-md w-full"
            style={{
              background: "rgba(255, 255, 255, 0.90)",
              border: "1px solid #54C571",
            }}
          >
            <h1
              className="text-5xl font-extrabold mb-4 font-poppins"
              style={{ color: "#54C571" }}
            >
              No Access
            </h1>
            <h2
              className="text-2xl font-bold mb-6 font-poppins"
              style={{ color: "#82CAFF" }}
            >
              Permission Denied
            </h2>
            <p className="mb-6 font-poppins" style={{ color: "#555" }}>
              You do not have permission to view this page.<br />
              Required role:{" "}
              <span className="font-semibold" style={{ color: "#D462FF" }}>
                {rolesAllowed.join(", ")}
              </span>
            </p>
            <button
              onClick={handleLogout}
              className="inline-block px-6 py-3 rounded-xl transition font-semibold font-poppins shadow-md"
              style={{
                background: "#54C571",
                color: "#fff",
                border: "1px solid #54C571",
              }}
              onMouseOver={e =>
                (e.currentTarget.style.background = "#D462FF")
              }
              onMouseOut={e =>
                (e.currentTarget.style.background = "#54C571")
              }
            >
              Log out
            </button>
          </div>
        </main>
      );
    }
  }

  return <>{children}</>;
};