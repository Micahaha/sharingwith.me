import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Plus, Download, Linkedin, Github } from "lucide-react"

export default function Page() {
  return (
    <div className="flex min-h-svh items-center justify-center bg-background px-4">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground text-center">ShareWith.me</h1>
          <p className="text-sm text-muted-foreground">
            Quickly send or receive files with anyone using a simple key.
          </p>
        </div>

        <Card className="cursor-pointer transition-shadow hover:shadow-md">
          <CardContent className="flex flex-col gap-2 p-6">
            <p className="font-semibold text-foreground text-center">Send</p>
            <div className="flex items-center justify-center py-6">
              <Plus className="h-10 w-10 text-primary" strokeWidth={1.5} />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="flex flex-col gap-2 p-6">
            <p className="font-semibold text-foreground text-center pb-5">Receive</p>
            <div className="relative">
              <Input placeholder="Input key" className="pr-10" />
              <Download className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            </div>
          </CardContent>
        </Card>

        <div className="flex justify-center gap-2">
          <Button variant="ghost" size="icon" asChild>
            <a href="https://linkedin.com" target="_blank" rel="noopener noreferrer" aria-label="LinkedIn">
              <Linkedin className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
          <Button variant="ghost" size="icon" asChild>
            <a href="https://github.com" target="_blank" rel="noopener noreferrer" aria-label="GitHub">
              <Github className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
        </div>
      </div>
    </div>
  )
}
