import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./Pages/Home";
import Players from "./Pages/Players";
import Player from "./Pages/Player";
import Profile from "./Pages/Profile";
import Status from "./Pages/Status";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />}>
          <Route index element={<Home />} />
          <Route path="*" element={<Home />} />
        </Route>
        <Route path="players" element={<Players />} />
        <Route path="player/:id" element={<Player />} />
        <Route path="profile" element={<Profile />} />
        <Route path="status" element={<Status />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
