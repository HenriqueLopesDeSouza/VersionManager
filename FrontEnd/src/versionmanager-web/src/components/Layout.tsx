import { Link, Outlet } from "react-router-dom";

export function Layout() {
  return (
    <>
      <nav className="navbar navbar-expand navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand" to="/softwares">
            VersionManager
          </Link>
          <div className="navbar-nav">
            <Link className="nav-link" to="/softwares">
              Softwares
            </Link>
          </div>
        </div>
      </nav>

      <main className="container py-4">
        <Outlet />
      </main>
    </>
  );
}
