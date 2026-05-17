import { useMemo } from "react";
import StatsCard from "../components/StatsCard";

export default function GoalsProgress({ subject }) {

  const chartData = useMemo(() => {
    if (!subject || !subject.studyGoals) return [];

    return subject.studyGoals.map((goal, index) => {
      const percent = goal.targetHours
        ? Math.round((goal.currentHours / goal.targetHours) * 100)
        : 0;

      return {
        name: `G${index + 1}`,
        value: percent, 
      };
    });
  }, [subject]);

  return (
    <div className="p-6">
      {!subject ? (
        <p>No subject selected</p>
      ) : subject.studyGoals.length === 0 ? (
        <p>No goals for this subject</p>
      ) : (
        <StatsCard data={chartData} />
      )}
    </div>
  );
}