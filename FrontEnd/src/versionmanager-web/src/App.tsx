import { Routes, Route, Navigate } from "react-router-dom";
import { Layout } from "./components/Layout";

import { SoftwareList } from "./pages/softwares/SoftwareList";
import { SoftwareForm } from "./pages/softwares/SoftwareForm";
import { VersionList } from "./pages/versions/VersionList";
import { VersionForm } from "./pages/versions/VersionForm";

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Navigate to="/softwares" />} />

        <Route path="/softwares" element={<SoftwareList />} />
        <Route path="/softwares/new" element={<SoftwareForm mode="create" />} />
        <Route path="/softwares/:id/edit" element={<SoftwareForm mode="edit" />} />

        <Route path="/softwares/:softwareId/versions" element={<VersionList />} />
        <Route path="/softwares/:softwareId/versions/new" element={<VersionForm mode="create" />} />
        <Route path="/softwares/:softwareId/versions/:versionId/edit" element={<VersionForm mode="edit" />} />
      </Route>
    </Routes>
  );
}
